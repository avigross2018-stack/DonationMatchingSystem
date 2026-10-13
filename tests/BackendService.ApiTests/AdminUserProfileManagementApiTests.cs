using BackendService.ApiTests.Infrastructure;
using BackendService.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendService.ApiTests;

public sealed class AdminUserProfileManagementApiTests(
    ApiTestFixture fixture) : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task UpdateDonor_ValidPayload_TrimsAndPersistsProfile()
    {
        var admin = await Fixture.SeedUserAsync(
            "ADMIN",
            "+972533600001");

        var donor = await Fixture.SeedUserAsync(
            "DONOR",
            "+972533600002");

        var donorId = await Fixture.WithDbAsync(db =>
            db.Donors
                .Where(x => x.UserId == donor.Id)
                .Select(x => x.Id)
                .SingleAsync());

        using var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"/api/admin/donors/{donorId}")
        {
            Content = JsonContent.Create(new
            {
                name = "  Updated Donor  ",
                phone = "+972533600003"
            })
        }
            .WithBearer(await Fixture.LoginAsync(admin.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.ReadJsonAsync();

        Assert.Equal(
            donorId,
            json.GetProperty("id").GetInt32());

        Assert.Equal(
            "Updated Donor",
            json.GetProperty("name").GetString());

        Assert.Equal(
            "+972533600003",
            json.GetProperty("phone").GetString());

        Assert.True(json.GetProperty("isActive").GetBoolean());

        await Fixture.WithDbAsync(async db =>
        {
            var persisted = await db.Users
                .AsNoTracking()
                .SingleAsync(x => x.Id == donor.Id);

            Assert.Equal("Updated Donor", persisted.Name);
            Assert.Equal("+972533600003", persisted.Phone);
            Assert.True(persisted.IsActive);
        });
    }

    [Fact]
    public async Task UpdateDonor_PhoneAlreadyExists_ReturnsConflictWithoutChanges()
    {
        var admin = await Fixture.SeedUserAsync(
            "ADMIN",
            "+972533600011");

        var targetDonor = await Fixture.SeedUserAsync(
            "DONOR",
            "+972533600012");

        var otherDonor = await Fixture.SeedUserAsync(
            "DONOR",
            "+972533600013");

        var targetDonorId = await Fixture.WithDbAsync(db =>
            db.Donors
                .Where(x => x.UserId == targetDonor.Id)
                .Select(x => x.Id)
                .SingleAsync());

        using var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"/api/admin/donors/{targetDonorId}")
        {
            Content = JsonContent.Create(new
            {
                name = "Must Not Be Saved",
                phone = otherDonor.Phone
            })
        }
            .WithBearer(await Fixture.LoginAsync(admin.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        await Fixture.WithDbAsync(async db =>
        {
            var persisted = await db.Users
                .AsNoTracking()
                .SingleAsync(x => x.Id == targetDonor.Id);

            Assert.Equal("Test DONOR", persisted.Name);
            Assert.Equal(targetDonor.Phone, persisted.Phone);
            Assert.True(persisted.IsActive);
        });
    }

    [Fact]
    public async Task GetDonorById_ReturnsDonorWithNewestDonationsFirst()
    {
        var population = await Fixture.SeedPopulationTypeAsync();

        var admin = await Fixture.SeedUserAsync(
            "ADMIN",
            "+972533600021");

        var donor = await Fixture.SeedUserAsync(
            "DONOR",
            "+972533600022");

        var donorId = await Fixture.WithDbAsync(db =>
            db.Donors
                .Where(x => x.UserId == donor.Id)
                .Select(x => x.Id)
                .SingleAsync());

        var olderDate = DateTime.UtcNow.AddDays(-2);
        var newerDate = DateTime.UtcNow.AddDays(-1);

        await CreateDonationAsync(
            donor.Id,
            population.Id,
            "Older donation",
            300m,
            200m,
            olderDate);

        await CreateDonationAsync(
            donor.Id,
            population.Id,
            "Newer donation",
            500m,
            400m,
            newerDate);

        using var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/admin/donors/{donorId}")
            .WithBearer(await Fixture.LoginAsync(admin.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.ReadJsonAsync();

        Assert.Equal(donorId, json.GetProperty("id").GetInt32());
        Assert.Equal(donor.Id, json.GetProperty("userId").GetInt32());
        Assert.Equal("Test DONOR", json.GetProperty("name").GetString());
        Assert.Equal(donor.Phone, json.GetProperty("phone").GetString());
        Assert.True(json.GetProperty("isActive").GetBoolean());

        var donations = json.GetProperty("donations").EnumerateArray().ToArray();

        Assert.Equal(2, donations.Length);

        Assert.Equal(
            "Newer donation",
            donations[0].GetProperty("description").GetString());

        Assert.Equal(
            "Older donation",
            donations[1].GetProperty("description").GetString());

        Assert.Equal(
            population.Id,
            donations[0].GetProperty("populationTypeId").GetInt32());

        Assert.Equal(
            population.Name,
            donations[0].GetProperty("populationTypeName").GetString());

        Assert.Equal(
            500m,
            donations[0].GetProperty("totalAmount").GetDecimal());

        Assert.Equal(
            400m,
            donations[0].GetProperty("remainingAmount").GetDecimal());
    }

    [Fact]
    public async Task GetRecipientById_ReturnsRecipientWithNewestRedemptionsFirst()
    {
        var population = await Fixture.SeedPopulationTypeAsync();

        var admin = await Fixture.SeedUserAsync(
            "ADMIN",
            "+972533600031");

        var donor = await Fixture.SeedUserAsync(
            "DONOR",
            "+972533600032");

        var recipientUser = await Fixture.SeedUserAsync(
            "RECIPIENT",
            "+972533600033",
            populationTypeId: population.Id,
            eligibility: 1_000m);

        var recipientId = await Fixture.WithDbAsync(async db =>
        {
            var recipient = await db.Recipients
                .SingleAsync(x => x.UserId == recipientUser.Id);

            recipient.RequestReason = "Support for family expenses";

            await db.SaveChangesAsync();

            return recipient.Id;
        });

        var donationId = await CreateDonationAsync(
            donor.Id,
            population.Id,
            "Recipient history donation",
            500m,
            400m,
            DateTime.UtcNow.AddDays(-1));

        var olderRedemptionDate = DateTime.UtcNow.AddMinutes(-2);
        var newerRedemptionDate = DateTime.UtcNow.AddMinutes(-1);

        await Fixture.WithDbAsync(async db =>
        {
            db.Redemptions.AddRange(
                new Redemption
                {
                    DonationId = donationId,
                    RecipientId = recipientId,
                    Amount = 50m,
                    Status = "PENDING",
                    CreatedAt = olderRedemptionDate
                },
                new Redemption
                {
                    DonationId = donationId,
                    RecipientId = recipientId,
                    Amount = 75m,
                    Status = "COMPLETED",
                    CreatedAt = newerRedemptionDate,
                    RedeemedAt = newerRedemptionDate.AddSeconds(10)
                });

            await db.SaveChangesAsync();
        });

        using var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/admin/recipients/{recipientId}")
            .WithBearer(await Fixture.LoginAsync(admin.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.ReadJsonAsync();

        Assert.Equal(
            recipientId,
            json.GetProperty("id").GetInt32());

        Assert.Equal(
            recipientUser.Id,
            json.GetProperty("userId").GetInt32());

        Assert.Equal(
            "Test RECIPIENT",
            json.GetProperty("name").GetString());

        Assert.Equal(
            population.Id,
            json.GetProperty("populationTypeId").GetInt32());

        Assert.Equal(
            population.Name,
            json.GetProperty("populationTypeName").GetString());

        Assert.Equal(
            "Support for family expenses",
            json.GetProperty("requestReason").GetString());

        var redemptions = json.GetProperty("redemptions").EnumerateArray().ToArray();

        Assert.Equal(2, redemptions.Length);

        Assert.Equal(
            "COMPLETED",
            redemptions[0].GetProperty("status").GetString());

        Assert.Equal(
            75m,
            redemptions[0].GetProperty("amount").GetDecimal());

        Assert.Equal(
            "Test DONOR",
            redemptions[0].GetProperty("donorName").GetString());

        Assert.Equal(
            "Recipient history donation",
            redemptions[0].GetProperty("donationDescription").GetString());

        Assert.Equal(
            "PENDING",
            redemptions[1].GetProperty("status").GetString());

        Assert.Equal(
            50m,
            redemptions[1].GetProperty("amount").GetDecimal());
    }

    private async Task<int> CreateDonationAsync(
        int donorUserId,
        int populationTypeId,
        string description,
        decimal totalAmount,
        decimal remainingAmount,
        DateTime createdAt)
    {
        return await Fixture.WithDbAsync(async db =>
        {
            var donorId = await db.Donors
                .Where(x => x.UserId == donorUserId)
                .Select(x => x.Id)
                .SingleAsync();

            var donation = new Donation
            {
                DonorId = donorId,
                PopulationTypeId = populationTypeId,
                TotalAmount = totalAmount,
                RemainingAmount = remainingAmount,
                MaxAmountPerRecipient = 100m,
                Description = description,
                CreatedAt = createdAt,
                ExpirationDate = createdAt.AddDays(30),
                Status = "ACTIVE"
            };

            db.Donations.Add(donation);
            await db.SaveChangesAsync();

            return donation.Id;
        });
    }
}
