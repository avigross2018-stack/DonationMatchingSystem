using BackendService.ApiTests.Infrastructure;
using BackendService.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendService.ApiTests;

public sealed class RecipientProfileApiTests(ApiTestFixture fixture)
    : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task GetMyProfile_ActiveRecipient_ReturnsIdentityPopulationAndEligibility()
    {
        var population = await Fixture.SeedPopulationTypeAsync(name: "Families");
        var user = await Fixture.SeedUserAsync(
            "RECIPIENT", "+972532000001", populationTypeId: population.Id, eligibility: 1_250m);
        await Fixture.WithDbAsync(async db =>
        {
            var recipient = await db.Recipients.SingleAsync(x => x.UserId == user.Id);
            recipient.IncomeAmount = 750m;
            recipient.AmountOfChildren = 3;
            recipient.RequestReason = "Profile reason";
            await db.SaveChangesAsync();
        });
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/recipients/me")
            .WithBearer(await Fixture.LoginAsync(user.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.ReadJsonAsync();
        Assert.Equal(user.Name, json.GetProperty("name").GetString());
        Assert.Equal(user.Phone, json.GetProperty("phone").GetString());
        Assert.Equal(population.Id, json.GetProperty("populationTypeId").GetInt32());
        Assert.Equal("Families", json.GetProperty("populationTypeName").GetString());
        Assert.Equal(750m, json.GetProperty("incomeAmount").GetDecimal());
        Assert.Equal(3, json.GetProperty("amountOfChildren").GetInt32());
        Assert.Equal("Profile reason", json.GetProperty("requestReason").GetString());
        Assert.Equal(1_250m, json.GetProperty("eligibilityAmount").GetDecimal());
        Assert.Equal(1_250m, json.GetProperty("remainingEligibility").GetDecimal());
    }

    [Fact]
    public async Task UpdateMyProfile_ValidPayload_RecalculatesEligibilityAndSubtractsCompletedTotal()
    {
        var population = await Fixture.SeedPopulationTypeAsync(baseAmount: 1_000m);
        var recipientUser = await Fixture.SeedUserAsync(
            "RECIPIENT", "+972532000002", populationTypeId: population.Id, eligibility: 900m);
        var donorUser = await Fixture.SeedUserAsync("DONOR", "+972532000003");
        var recipientId = await SeedCompletedRedemptionAsync(
            donorUser.Id,
            recipientUser.Id,
            population.Id,
            300m);
        using var request = CreateUpdateRequest(
            await Fixture.LoginAsync(recipientUser.Phone),
            1_000m,
            2,
            "Updated reason");

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        await Fixture.WithDbAsync(async db =>
        {
            var recipient = await db.Recipients.AsNoTracking()
                .SingleAsync(x => x.Id == recipientId);
            Assert.Equal(1_000m, recipient.IncomeAmount);
            Assert.Equal(2, recipient.AmountOfChildren);
            Assert.Equal("Updated reason", recipient.RequestReason);
            Assert.Equal(1_800m, recipient.EligibilityAmount);
            Assert.Equal(1_500m, recipient.RemainingEligibility);
            Assert.Equal(population.Id, recipient.PopulationTypeId);
        });
    }

    [Fact]
    public async Task UpdateMyProfile_IncomeWithThreeDecimalPlaces_ReturnsBadRequestWithoutChanges()
    {
        var population = await Fixture.SeedPopulationTypeAsync();
        var recipientUser = await Fixture.SeedUserAsync(
            "RECIPIENT", "+972532000004", populationTypeId: population.Id, eligibility: 1_000m);
        using var request = CreateUpdateRequest(
            await Fixture.LoginAsync(recipientUser.Phone),
            10.123m,
            1,
            "Must not persist");

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var persisted = await Fixture.WithDbAsync(db => db.Recipients.AsNoTracking()
            .SingleAsync(x => x.UserId == recipientUser.Id));
        Assert.Equal(0m, persisted.IncomeAmount);
        Assert.Equal(0, persisted.AmountOfChildren);
        Assert.Null(persisted.RequestReason);
        Assert.Equal(1_000m, persisted.EligibilityAmount);
        Assert.Equal(1_000m, persisted.RemainingEligibility);
    }

    [Fact]
    public async Task GetMyRedemptions_ReturnsOnlyAuthenticatedRecipientsHistoryAndDtoFields()
    {
        var population = await Fixture.SeedPopulationTypeAsync();
        var donor = await Fixture.SeedUserAsync("DONOR", "+972532000005");
        var owner = await Fixture.SeedUserAsync(
            "RECIPIENT", "+972532000006", populationTypeId: population.Id);
        var other = await Fixture.SeedUserAsync(
            "RECIPIENT", "+972532000007", populationTypeId: population.Id);
        var ownRedemptionId = await SeedHistoryAsync(
            donor.Id,
            owner.Id,
            other.Id,
            population.Id);
        using var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/api/recipients/me/redemptions")
            .WithBearer(await Fixture.LoginAsync(owner.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var item = Assert.Single((await response.ReadJsonAsync()).EnumerateArray());
        Assert.Equal(ownRedemptionId, item.GetProperty("id").GetInt32());
        Assert.Equal("PENDING", item.GetProperty("status").GetString());
        Assert.Equal("For history", item.GetProperty("donationDescription").GetString());
        Assert.Equal(
            new[] { "amount", "createdAt", "donationDescription", "donationId", "id", "redeemedAt", "status" },
            item.EnumerateObject().Select(x => x.Name).Order().ToArray());
        Assert.DoesNotContain("password", item.GetRawText(), StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("phone", item.GetRawText(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeactivateMyAccount_ActiveRecipient_CancelsOnlyOwnPendingRedemptions()
    {
        var population = await Fixture.SeedPopulationTypeAsync();
        var donor = await Fixture.SeedUserAsync("DONOR", "+972532000008");
        var recipient = await Fixture.SeedUserAsync(
            "RECIPIENT", "+972532000009", populationTypeId: population.Id);
        var seededStatuses = await SeedRecipientDeactivationGraphAsync(
            donor.Id,
            recipient.Id,
            population.Id);
        var token = await Fixture.LoginAsync(recipient.Phone);
        using var request = new HttpRequestMessage(HttpMethod.Delete, "/api/recipients/me")
            .WithBearer(token);

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        await Fixture.WithDbAsync(async db =>
        {
            Assert.False(await db.Users.Where(x => x.Id == recipient.Id)
                .Select(x => x.IsActive).SingleAsync());
            var statuses = await db.Redemptions.AsNoTracking()
                .Where(x => seededStatuses.Contains(x.Id))
                .OrderBy(x => x.Id)
                .Select(x => x.Status)
                .ToListAsync();
            Assert.Equal(new[] { "CANCELLED", "COMPLETED", "REJECTED", "CANCELLED" }, statuses);
        });

        using var afterDeactivation = new HttpRequestMessage(HttpMethod.Get, "/api/recipients/me")
            .WithBearer(token);
        using var rejected = await Client.SendAsync(afterDeactivation);
        Assert.Equal(HttpStatusCode.Unauthorized, rejected.StatusCode);
    }

    private async Task<int> SeedCompletedRedemptionAsync(
        int donorUserId,
        int recipientUserId,
        int populationTypeId,
        decimal amount)
    {
        return await Fixture.WithDbAsync(async db =>
        {
            var donation = await AddDonationAsync(db, donorUserId, populationTypeId, "CANCELLED");
            var recipientId = await db.Recipients.Where(x => x.UserId == recipientUserId)
                .Select(x => x.Id).SingleAsync();
            db.Redemptions.Add(new Redemption
            {
                DonationId = donation.Id,
                RecipientId = recipientId,
                Amount = amount,
                Status = "COMPLETED",
                CreatedAt = DateTime.UtcNow,
                RedeemedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
            return recipientId;
        });
    }

    private async Task<int> SeedHistoryAsync(
        int donorUserId,
        int ownerUserId,
        int otherUserId,
        int populationTypeId)
    {
        return await Fixture.WithDbAsync(async db =>
        {
            var donation = await AddDonationAsync(db, donorUserId, populationTypeId, "CANCELLED", "For history");
            var recipientIds = await db.Recipients
                .Where(x => x.UserId == ownerUserId || x.UserId == otherUserId)
                .ToDictionaryAsync(x => x.UserId, x => x.Id);
            var own = new Redemption
            {
                DonationId = donation.Id,
                RecipientId = recipientIds[ownerUserId],
                Amount = 40m,
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow
            };
            db.Redemptions.AddRange(own, new Redemption
            {
                DonationId = donation.Id,
                RecipientId = recipientIds[otherUserId],
                Amount = 60m,
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow.AddMinutes(1)
            });
            await db.SaveChangesAsync();
            return own.Id;
        });
    }

    private async Task<int[]> SeedRecipientDeactivationGraphAsync(
        int donorUserId,
        int recipientUserId,
        int populationTypeId)
    {
        return await Fixture.WithDbAsync(async db =>
        {
            var donation = await AddDonationAsync(db, donorUserId, populationTypeId, "CANCELLED");
            var recipientId = await db.Recipients.Where(x => x.UserId == recipientUserId)
                .Select(x => x.Id).SingleAsync();
            var rows = new[] { "PENDING", "COMPLETED", "REJECTED", "CANCELLED" }
                .Select(status => new Redemption
                {
                    DonationId = donation.Id,
                    RecipientId = recipientId,
                    Amount = 20m,
                    Status = status,
                    CreatedAt = DateTime.UtcNow
                })
                .ToArray();
            db.Redemptions.AddRange(rows);
            await db.SaveChangesAsync();
            return rows.Select(x => x.Id).ToArray();
        });
    }

    private static async Task<Donation> AddDonationAsync(
        BackendService.Data.AppDbContext db,
        int donorUserId,
        int populationTypeId,
        string status,
        string description = "Recipient test")
    {
        var donorId = await db.Donors.Where(x => x.UserId == donorUserId)
            .Select(x => x.Id).SingleAsync();
        var donation = new Donation
        {
            DonorId = donorId,
            PopulationTypeId = populationTypeId,
            TotalAmount = 1_000m,
            RemainingAmount = 1_000m,
            MaxAmountPerRecipient = 500m,
            Description = description,
            Status = status,
            CreatedAt = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddDays(2)
        };
        db.Donations.Add(donation);
        await db.SaveChangesAsync();
        return donation;
    }

    private static HttpRequestMessage CreateUpdateRequest(
        string token,
        decimal incomeAmount,
        int amountOfChildren,
        string requestReason) =>
        new HttpRequestMessage(HttpMethod.Put, "/api/recipients/me")
        {
            Content = JsonContent.Create(new { incomeAmount, amountOfChildren, requestReason })
        }.WithBearer(token);
}
