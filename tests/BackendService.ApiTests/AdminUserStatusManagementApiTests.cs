using BackendService.ApiTests.Infrastructure;
using BackendService.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendService.ApiTests;

public sealed class AdminUserStatusManagementApiTests(
    ApiTestFixture fixture) : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task DeactivateDonor_CancelsActiveDonationsAndPendingRedemptions()
    {
        var population = await Fixture.SeedPopulationTypeAsync();

        var admin = await Fixture.SeedUserAsync(
            "ADMIN",
            "+972533500001");

        var donor = await Fixture.SeedUserAsync(
            "DONOR",
            "+972533500002");

        var recipientUser = await Fixture.SeedUserAsync(
            "RECIPIENT",
            "+972533500003",
            populationTypeId: population.Id);

        var recipientId = await Fixture.WithDbAsync(db =>
            db.Recipients
                .Where(x => x.UserId == recipientUser.Id)
                .Select(x => x.Id)
                .SingleAsync());

        var donorId = await Fixture.WithDbAsync(db =>
            db.Donors
                .Where(x => x.UserId == donor.Id)
                .Select(x => x.Id)
                .SingleAsync());

        var donationId = await CreateDonationAsync(
            donor.Id,
            population.Id);

        await Fixture.WithDbAsync(async db =>
        {
            db.Redemptions.AddRange(
                new Redemption
                {
                    DonationId = donationId,
                    RecipientId = recipientId,
                    Amount = 25m,
                    Status = "PENDING",
                    CreatedAt = DateTime.UtcNow
                },
                new Redemption
                {
                    DonationId = donationId,
                    RecipientId = recipientId,
                    Amount = 25m,
                    Status = "COMPLETED",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-2),
                    RedeemedAt = DateTime.UtcNow.AddMinutes(-1)
                },
                new Redemption
                {
                    DonationId = donationId,
                    RecipientId = recipientId,
                    Amount = 25m,
                    Status = "REJECTED",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-3)
                });

            await db.SaveChangesAsync();
        });

        using var request = new HttpRequestMessage(
                HttpMethod.Patch,
                $"/api/admin/donors/{donorId}/status")
        {
            Content = JsonContent.Create(new
            {
                isActive = false
            })
        }
            .WithBearer(await Fixture.LoginAsync(admin.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.ReadJsonAsync();

        Assert.Equal(
            donorId,
            json.GetProperty("donorId").GetInt32());

        Assert.False(json.GetProperty("isActive").GetBoolean());

        Assert.Equal(
            1,
            json.GetProperty("cancelledDonationsCount").GetInt32());

        Assert.Equal(
            1,
            json.GetProperty("cancelledRedemptionsCount").GetInt32());

        await Fixture.WithDbAsync(async db =>
        {
            var persistedUser = await db.Users
                .AsNoTracking()
                .SingleAsync(x => x.Id == donor.Id);

            Assert.False(persistedUser.IsActive);

            var donation = await db.Donations
                .AsNoTracking()
                .SingleAsync(x => x.Id == donationId);

            Assert.Equal("CANCELLED", donation.Status);

            // Deactivation must not modify financial balances.
            Assert.Equal(500m, donation.TotalAmount);
            Assert.Equal(400m, donation.RemainingAmount);

            var redemptions = await db.Redemptions
                .AsNoTracking()
                .Where(x => x.DonationId == donationId)
                .ToListAsync();

            Assert.Single(redemptions, x => x.Status == "CANCELLED");
            Assert.Single(redemptions, x => x.Status == "COMPLETED");
            Assert.Single(redemptions, x => x.Status == "REJECTED");
        });
    }

    [Fact]
    public async Task DeactivateRecipient_CancelsOnlyTheirPendingRedemptions()
    {
        var population = await Fixture.SeedPopulationTypeAsync();

        var admin = await Fixture.SeedUserAsync(
            "ADMIN",
            "+972533500011");

        var donor = await Fixture.SeedUserAsync(
            "DONOR",
            "+972533500012");

        var recipientUser = await Fixture.SeedUserAsync(
            "RECIPIENT",
            "+972533500013",
            populationTypeId: population.Id);

        var otherRecipientUser = await Fixture.SeedUserAsync(
            "RECIPIENT",
            "+972533500014",
            populationTypeId: population.Id);

        var recipientId = await Fixture.WithDbAsync(db =>
            db.Recipients
                .Where(x => x.UserId == recipientUser.Id)
                .Select(x => x.Id)
                .SingleAsync());

        var otherRecipientId = await Fixture.WithDbAsync(db =>
            db.Recipients
                .Where(x => x.UserId == otherRecipientUser.Id)
                .Select(x => x.Id)
                .SingleAsync());

        var donationId = await CreateDonationAsync(
            donor.Id,
            population.Id);

        await Fixture.WithDbAsync(async db =>
        {
            db.Redemptions.AddRange(
                new Redemption
                {
                    DonationId = donationId,
                    RecipientId = recipientId,
                    Amount = 25m,
                    Status = "PENDING",
                    CreatedAt = DateTime.UtcNow
                },
                new Redemption
                {
                    DonationId = donationId,
                    RecipientId = recipientId,
                    Amount = 25m,
                    Status = "COMPLETED",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-2),
                    RedeemedAt = DateTime.UtcNow.AddMinutes(-1)
                },
                new Redemption
                {
                    DonationId = donationId,
                    RecipientId = recipientId,
                    Amount = 25m,
                    Status = "REJECTED",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-3)
                },
                new Redemption
                {
                    DonationId = donationId,
                    RecipientId = otherRecipientId,
                    Amount = 25m,
                    Status = "PENDING",
                    CreatedAt = DateTime.UtcNow
                });

            await db.SaveChangesAsync();
        });

        using var request = new HttpRequestMessage(
                HttpMethod.Patch,
                $"/api/admin/recipients/{recipientId}/status")
        {
            Content = JsonContent.Create(new
            {
                isActive = false
            })
        }
            .WithBearer(await Fixture.LoginAsync(admin.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.ReadJsonAsync();

        Assert.Equal(
            recipientId,
            json.GetProperty("recipientId").GetInt32());

        Assert.False(json.GetProperty("isActive").GetBoolean());

        Assert.Equal(
            1,
            json.GetProperty("cancelledRedemptionsCount").GetInt32());

        await Fixture.WithDbAsync(async db =>
        {
            var persistedUser = await db.Users
                .AsNoTracking()
                .SingleAsync(x => x.Id == recipientUser.Id);

            Assert.False(persistedUser.IsActive);

            var donation = await db.Donations
                .AsNoTracking()
                .SingleAsync(x => x.Id == donationId);

            // Deactivating a recipient must not cancel the donation.
            Assert.Equal("ACTIVE", donation.Status);
            Assert.Equal(500m, donation.TotalAmount);
            Assert.Equal(400m, donation.RemainingAmount);

            var redemptions = await db.Redemptions
                .AsNoTracking()
                .Where(x => x.DonationId == donationId)
                .ToListAsync();

            Assert.Single(
                redemptions,
                x => x.RecipientId == recipientId &&
                     x.Status == "CANCELLED");

            Assert.Single(
                redemptions,
                x => x.RecipientId == recipientId &&
                     x.Status == "COMPLETED");

            Assert.Single(
                redemptions,
                x => x.RecipientId == recipientId &&
                     x.Status == "REJECTED");

            // Other recipients' requests must remain unchanged.
            Assert.Single(
                redemptions,
                x => x.RecipientId == otherRecipientId &&
                     x.Status == "PENDING");
        });
    }

    private async Task<int> CreateDonationAsync(
        int donorUserId,
        int populationTypeId)
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
                TotalAmount = 500m,
                RemainingAmount = 400m,
                MaxAmountPerRecipient = 100m,
                Description = "Admin user status test",
                CreatedAt = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(3),
                Status = "ACTIVE"
            };

            db.Donations.Add(donation);
            await db.SaveChangesAsync();

            return donation.Id;
        });
    }
}
