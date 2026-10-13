using BackendService.ApiTests.Infrastructure;
using BackendService.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendService.ApiTests;

public sealed class AdminDashboardStatisticsApiTests(
    ApiTestFixture fixture) : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task GetDashboardStatistics_Admin_ReturnsAggregatedStatistics()
    {
        var activePopulation = await Fixture.SeedPopulationTypeAsync(
            active: true,
            baseAmount: 1_000m);

        var inactivePopulation = await Fixture.SeedPopulationTypeAsync(
            active: false,
            baseAmount: 2_000m);

        var admin = await Fixture.SeedUserAsync(
            "ADMIN", "+972533300001");

        var activeDonor = await Fixture.SeedUserAsync(
            "DONOR", "+972533300002");

        var inactiveDonor = await Fixture.SeedUserAsync(
            "DONOR", "+972533300003", active: false);

        var activeRecipient = await Fixture.SeedUserAsync(
            "RECIPIENT",
            "+972533300004",
            populationTypeId: activePopulation.Id);

        var inactiveRecipient = await Fixture.SeedUserAsync(
            "RECIPIENT",
            "+972533300005",
            active: false,
            populationTypeId: inactivePopulation.Id);

        await Fixture.WithDbAsync(async db =>
        {
            var activeDonorId = await db.Donors
                .Where(x => x.UserId == activeDonor.Id)
                .Select(x => x.Id)
                .SingleAsync();

            var activeDonation = new Donation
            {
                DonorId = activeDonorId,
                PopulationTypeId = activePopulation.Id,
                TotalAmount = 500m,
                RemainingAmount = 300m,
                MaxAmountPerRecipient = 200m,
                Description = "Active donation",
                CreatedAt = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(3),
                Status = "ACTIVE"
            };

            var completedDonation = new Donation
            {
                DonorId = activeDonorId,
                PopulationTypeId = activePopulation.Id,
                TotalAmount = 200m,
                RemainingAmount = 0m,
                MaxAmountPerRecipient = 100m,
                Description = "Completed donation",
                CreatedAt = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(3),
                Status = "COMPLETED"
            };

            var cancelledDonation = new Donation
            {
                DonorId = activeDonorId,
                PopulationTypeId = inactivePopulation.Id,
                TotalAmount = 150m,
                RemainingAmount = 50m,
                MaxAmountPerRecipient = 50m,
                Description = "Cancelled donation",
                CreatedAt = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(3),
                Status = "CANCELLED"
            };

            db.Donations.AddRange(
                activeDonation,
                completedDonation,
                cancelledDonation);

            await db.SaveChangesAsync();

            var activeRecipientId = await db.Recipients
                .Where(x => x.UserId == activeRecipient.Id)
                .Select(x => x.Id)
                .SingleAsync();

            var inactiveRecipientId = await db.Recipients
                .Where(x => x.UserId == inactiveRecipient.Id)
                .Select(x => x.Id)
                .SingleAsync();

            db.Redemptions.AddRange(
                new Redemption
                {
                    DonationId = activeDonation.Id,
                    RecipientId = activeRecipientId,
                    Amount = 100m,
                    Status = "PENDING",
                    CreatedAt = DateTime.UtcNow
                },
                new Redemption
                {
                    DonationId = activeDonation.Id,
                    RecipientId = activeRecipientId,
                    Amount = 80m,
                    Status = "COMPLETED",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-5),
                    RedeemedAt = DateTime.UtcNow.AddMinutes(-4)
                },
                new Redemption
                {
                    DonationId = completedDonation.Id,
                    RecipientId = activeRecipientId,
                    Amount = 40m,
                    Status = "REJECTED",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-3)
                },
                new Redemption
                {
                    DonationId = cancelledDonation.Id,
                    RecipientId = inactiveRecipientId,
                    Amount = 50m,
                    Status = "CANCELLED",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-2)
                });

            await db.SaveChangesAsync();
        });

        using var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/api/admin/dashboard/statistics")
            .WithBearer(await Fixture.LoginAsync(admin.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.ReadJsonAsync();

        var users = json.GetProperty("users");

        Assert.Equal(2, users.GetProperty("donorsCount").GetInt32());
        Assert.Equal(2, users.GetProperty("recipientsCount").GetInt32());
        Assert.Equal(3, users.GetProperty("activeUsersCount").GetInt32());
        Assert.Equal(2, users.GetProperty("inactiveUsersCount").GetInt32());

        var donations = json.GetProperty("donations");

        Assert.Equal(3, donations.GetProperty("totalCount").GetInt32());
        Assert.Equal(
            850m,
            donations.GetProperty("totalDonationAmount").GetDecimal());
        Assert.Equal(
            1,
            donations.GetProperty("availableDonationCount").GetInt32());
        Assert.Equal(
            300m,
            donations.GetProperty("availableDonationBalance").GetDecimal());
        Assert.Equal(1, donations.GetProperty("activeCount").GetInt32());
        Assert.Equal(1, donations.GetProperty("completedCount").GetInt32());
        Assert.Equal(1, donations.GetProperty("cancelledCount").GetInt32());
        Assert.Equal(0, donations.GetProperty("expiredCount").GetInt32());

        var redemptions = json.GetProperty("redemptions");

        Assert.Equal(4, redemptions.GetProperty("totalCount").GetInt32());
        Assert.Equal(1, redemptions.GetProperty("pendingCount").GetInt32());
        Assert.Equal(1, redemptions.GetProperty("completedCount").GetInt32());
        Assert.Equal(1, redemptions.GetProperty("rejectedCount").GetInt32());
        Assert.Equal(1, redemptions.GetProperty("cancelledCount").GetInt32());
        Assert.Equal(
            80m,
            redemptions.GetProperty("completedRedemptionAmount").GetDecimal());

        var populations = json.GetProperty("populationTypes")
            .EnumerateArray()
            .ToArray();

        Assert.Equal(2, populations.Length);

        var activePopulationStats = Assert.Single(
            populations,
            item => item.GetProperty("populationTypeId").GetInt32()
                == activePopulation.Id);

        Assert.True(activePopulationStats.GetProperty("isActive").GetBoolean());
        Assert.Equal(
            1,
            activePopulationStats.GetProperty("recipientsCount").GetInt32());
        Assert.Equal(
            2,
            activePopulationStats.GetProperty("donationsCount").GetInt32());
        Assert.Equal(
            700m,
            activePopulationStats.GetProperty("totalDonationAmount").GetDecimal());
        Assert.Equal(
            300m,
            activePopulationStats.GetProperty("availableDonationBalance").GetDecimal());
        Assert.Equal(
            1,
            activePopulationStats.GetProperty("pendingRedemptionsCount").GetInt32());
        Assert.Equal(
            1,
            activePopulationStats.GetProperty("completedRedemptionsCount").GetInt32());
        Assert.Equal(
            80m,
            activePopulationStats.GetProperty("completedRedemptionAmount").GetDecimal());

        var inactivePopulationStats = Assert.Single(
            populations,
            item => item.GetProperty("populationTypeId").GetInt32()
                == inactivePopulation.Id);

        Assert.False(
            inactivePopulationStats.GetProperty("isActive").GetBoolean());
        Assert.Equal(
            1,
            inactivePopulationStats.GetProperty("recipientsCount").GetInt32());
        Assert.Equal(
            1,
            inactivePopulationStats.GetProperty("donationsCount").GetInt32());
        Assert.Equal(
            150m,
            inactivePopulationStats.GetProperty("totalDonationAmount").GetDecimal());
        Assert.Equal(
            1,
            inactivePopulationStats.GetProperty("cancelledRedemptionsCount").GetInt32());

        Assert.True(
            json.TryGetProperty("generatedAtUtc", out _));
    }

    [Fact]
    public async Task GetDashboardStatistics_DonorToken_ReturnsForbidden()
    {
        var donor = await Fixture.SeedUserAsync(
            "DONOR", "+972533300011");

        using var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/api/admin/dashboard/statistics")
            .WithBearer(await Fixture.LoginAsync(donor.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetDashboardStatistics_Unauthenticated_ReturnsUnauthorized()
    {
        using var response = await Client.GetAsync(
            "/api/admin/dashboard/statistics");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
