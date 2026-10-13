using BackendService.ApiTests.Infrastructure;
using BackendService.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendService.ApiTests;

public sealed class AdminRecipientManagementApiTests(ApiTestFixture fixture)
    : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task UpdateRecipient_ValidPayload_RecalculatesEligibilityAndSubtractsCompletedTotal()
    {
        var population = await Fixture.SeedPopulationTypeAsync(baseAmount: 1_000m);
        var admin = await Fixture.SeedUserAsync("ADMIN", "+972533100001");
        var donor = await Fixture.SeedUserAsync("DONOR", "+972533100002");
        var recipientUser = await Fixture.SeedUserAsync(
            "RECIPIENT",
            "+972533100003",
            populationTypeId: population.Id,
            eligibility: 1_000m);

        var recipientId = await Fixture.WithDbAsync(db => db.Recipients
            .Where(x => x.UserId == recipientUser.Id)
            .Select(x => x.Id)
            .SingleAsync());

        await Fixture.WithDbAsync(async db =>
        {
            var donorId = await db.Donors
                .Where(x => x.UserId == donor.Id)
                .Select(x => x.Id)
                .SingleAsync();

            var donation = new Donation
            {
                DonorId = donorId,
                PopulationTypeId = population.Id,
                TotalAmount = 300m,
                RemainingAmount = 0m,
                MaxAmountPerRecipient = 300m,
                Description = "Completed historical redemption",
                CreatedAt = DateTime.UtcNow,
                Status = "CANCELLED"
            };

            db.Donations.Add(donation);
            await db.SaveChangesAsync();

            db.Redemptions.Add(new Redemption
            {
                DonationId = donation.Id,
                RecipientId = recipientId,
                Amount = 300m,
                Status = "COMPLETED",
                CreatedAt = DateTime.UtcNow,
                RedeemedAt = DateTime.UtcNow
            });

            await db.SaveChangesAsync();
        });

        using var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"/api/admin/recipients/{recipientId}")
        {
            Content = JsonContent.Create(new
            {
                name = "Updated Recipient",
                phone = "+972533100004",
                incomeAmount = 1_000m,
                amountOfChildren = 2,
                requestReason = "Updated reason",
                populationTypeId = population.Id + 100
            })
        }
            .WithBearer(await Fixture.LoginAsync(admin.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.ReadJsonAsync();

        Assert.Equal(
            "Updated Recipient",
            json.GetProperty("name").GetString());

        Assert.Equal(
            "+972533100004",
            json.GetProperty("phone").GetString());

        Assert.Equal(
            population.Id,
            json.GetProperty("populationTypeId").GetInt32());

        Assert.Equal(
            1_800m,
            json.GetProperty("eligibilityAmount").GetDecimal());

        Assert.Equal(
            1_500m,
            json.GetProperty("remainingEligibility").GetDecimal());

        await Fixture.WithDbAsync(async db =>
        {
            var recipient = await db.Recipients
                .AsNoTracking()
                .Include(x => x.User)
                .SingleAsync(x => x.Id == recipientId);

            Assert.Equal("Updated Recipient", recipient.User.Name);
            Assert.Equal("+972533100004", recipient.User.Phone);
            Assert.Equal(1_000m, recipient.IncomeAmount);
            Assert.Equal(2, recipient.AmountOfChildren);
            Assert.Equal("Updated reason", recipient.RequestReason);
            Assert.Equal(population.Id, recipient.PopulationTypeId);
            Assert.Equal(1_800m, recipient.EligibilityAmount);
            Assert.Equal(1_500m, recipient.RemainingEligibility);
        });
    }

    [Fact]
    public async Task UpdateRecipient_PhoneAlreadyExists_ReturnsConflictWithoutChanges()
    {
        var population = await Fixture.SeedPopulationTypeAsync();
        var admin = await Fixture.SeedUserAsync("ADMIN", "+972533100011");

        var targetUser = await Fixture.SeedUserAsync(
            "RECIPIENT",
            "+972533100012",
            populationTypeId: population.Id);

        var otherUser = await Fixture.SeedUserAsync(
            "RECIPIENT",
            "+972533100013",
            populationTypeId: population.Id);

        var recipientId = await Fixture.WithDbAsync(db => db.Recipients
            .Where(x => x.UserId == targetUser.Id)
            .Select(x => x.Id)
            .SingleAsync());

        using var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"/api/admin/recipients/{recipientId}")
        {
            Content = JsonContent.Create(new
            {
                name = "Must Not Be Saved",
                phone = otherUser.Phone,
                incomeAmount = 500m,
                amountOfChildren = 1,
                requestReason = "Must not be saved"
            })
        }
            .WithBearer(await Fixture.LoginAsync(admin.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        await Fixture.WithDbAsync(async db =>
        {
            var recipient = await db.Recipients
                .AsNoTracking()
                .Include(x => x.User)
                .SingleAsync(x => x.Id == recipientId);

            Assert.Equal("Test RECIPIENT", recipient.User.Name);
            Assert.Equal(targetUser.Phone, recipient.User.Phone);
            Assert.Equal(0m, recipient.IncomeAmount);
            Assert.Equal(0, recipient.AmountOfChildren);
            Assert.Null(recipient.RequestReason);
            Assert.Equal(1_000m, recipient.EligibilityAmount);
            Assert.Equal(1_000m, recipient.RemainingEligibility);
        });
    }

    [Fact]
    public async Task UpdateRecipient_IncomeWithMoreThanTwoDecimalPlaces_ReturnsBadRequest()
    {
        var population = await Fixture.SeedPopulationTypeAsync();
        var admin = await Fixture.SeedUserAsync("ADMIN", "+972533100021");

        var recipientUser = await Fixture.SeedUserAsync(
            "RECIPIENT",
            "+972533100022",
            populationTypeId: population.Id);

        var recipientId = await Fixture.WithDbAsync(db => db.Recipients
            .Where(x => x.UserId == recipientUser.Id)
            .Select(x => x.Id)
            .SingleAsync());

        using var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"/api/admin/recipients/{recipientId}")
        {
            Content = JsonContent.Create(new
            {
                name = "Updated Recipient",
                phone = "+972533100023",
                incomeAmount = 10.123m,
                amountOfChildren = 1,
                requestReason = "Invalid amount"
            })
        }
            .WithBearer(await Fixture.LoginAsync(admin.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var persisted = await Fixture.WithDbAsync(db => db.Recipients
            .AsNoTracking()
            .SingleAsync(x => x.Id == recipientId));

        Assert.Equal(0m, persisted.IncomeAmount);
        Assert.Equal(0, persisted.AmountOfChildren);
        Assert.Null(persisted.RequestReason);
        Assert.Equal(1_000m, persisted.EligibilityAmount);
        Assert.Equal(1_000m, persisted.RemainingEligibility);
    }
}
