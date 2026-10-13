using BackendService.ApiTests.Infrastructure;
using BackendService.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendService.ApiTests;

public sealed class AdminRedemptionApiTests(ApiTestFixture fixture)
    : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task GetCurrentAdmin_ActiveAdmin_ReturnsLinkedProfile()
    {
        var adminUser = await Fixture.SeedUserAsync("ADMIN", "+972533000001");
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/admin/me")
            .WithBearer(await Fixture.LoginAsync(adminUser.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.ReadJsonAsync();
        var adminId = await Fixture.WithDbAsync(db => db.Admins
            .Where(x => x.UserId == adminUser.Id)
            .Select(x => x.Id)
            .SingleAsync());
        Assert.Equal(adminId, json.GetProperty("adminId").GetInt32());
        Assert.Equal(adminUser.Id, json.GetProperty("userId").GetInt32());
        Assert.Equal(adminUser.Name, json.GetProperty("name").GetString());
        Assert.Equal(adminUser.Phone, json.GetProperty("phone").GetString());
        Assert.True(json.GetProperty("isActive").GetBoolean());
    }

    [Fact]
    public async Task GetRedemptions_StatusFilterAndPagination_ReturnsNestedAdminDto()
    {
        var scenario = await SeedScenarioAsync(
            "+972533000002",
            "+972533000003",
            "+972533000004",
            pendingCount: 2,
            includeCompleted: true);
        using var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/api/admin/redemptions?status=pending&page=1&pageSize=1")
            .WithBearer(await Fixture.LoginAsync(scenario.AdminPhone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await response.ReadJsonAsync();
        Assert.Equal(1, page.GetProperty("page").GetInt32());
        Assert.Equal(1, page.GetProperty("pageSize").GetInt32());
        Assert.Equal(2, page.GetProperty("totalCount").GetInt32());
        Assert.Equal(2, page.GetProperty("totalPages").GetInt32());
        var item = Assert.Single(page.GetProperty("items").EnumerateArray());
        Assert.Equal("PENDING", item.GetProperty("status").GetString());
        Assert.Equal("Test RECIPIENT", item.GetProperty("recipient").GetProperty("name").GetString());
        Assert.Equal("Test DONOR", item.GetProperty("donation").GetProperty("donorName").GetString());
        Assert.Equal("Admin test donation", item.GetProperty("donation").GetProperty("description").GetString());
        Assert.DoesNotContain("password", item.GetRawText(), StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("?page=0")]
    [InlineData("?pageSize=101")]
    [InlineData("?populationTypeId=0")]
    [InlineData("?status=unknown")]
    [InlineData("?fromDate=2026-10-14&toDate=2026-10-13")]
    public async Task GetRedemptions_InvalidQuery_ReturnsBadRequestProblemDetails(string query)
    {
        var admin = await Fixture.SeedUserAsync("ADMIN", "+972533000005");
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/admin/redemptions" + query)
            .WithBearer(await Fixture.LoginAsync(admin.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task ApproveRedemption_PendingAndBalancesAvailable_CompletesAtomicallyWithRecalculatedAmount()
    {
        var scenario = await SeedScenarioAsync(
            "+972533000006",
            "+972533000007",
            "+972533000008",
            pendingCount: 1,
            includeCompleted: true,
            donationRemaining: 700m,
            recipientRemaining: 450m,
            cap: 300m,
            completedAmount: 100m,
            pendingAmount: 250m);
        var pendingId = Assert.Single(scenario.PendingRedemptionIds);
        using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"/api/admin/redemptions/{pendingId}/approve")
            .WithBearer(await Fixture.LoginAsync(scenario.AdminPhone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.ReadJsonAsync();
        Assert.Equal(pendingId, json.GetProperty("redemptionId").GetInt32());
        Assert.Equal("COMPLETED", json.GetProperty("status").GetString());
        Assert.Equal(200m, json.GetProperty("approvedAmount").GetDecimal());
        Assert.Equal(500m, json.GetProperty("donationRemainingAmount").GetDecimal());
        Assert.Equal(250m, json.GetProperty("recipientRemainingEligibility").GetDecimal());

        await Fixture.WithDbAsync(async db =>
        {
            var redemption = await db.Redemptions.AsNoTracking().SingleAsync(x => x.Id == pendingId);
            Assert.Equal("COMPLETED", redemption.Status);
            Assert.Equal(200m, redemption.Amount);
            Assert.NotNull(redemption.RedeemedAt);
            Assert.Equal(scenario.AdminId, redemption.HandledByAdminId);
            Assert.Equal(500m, await db.Donations.Where(x => x.Id == scenario.DonationId)
                .Select(x => x.RemainingAmount).SingleAsync());
            Assert.Equal(250m, await db.Recipients.Where(x => x.Id == scenario.RecipientId)
                .Select(x => x.RemainingEligibility).SingleAsync());
        });
    }

    [Fact]
    public async Task RejectRedemption_Pending_SetsHandlerWithoutChangingBalances()
    {
        var scenario = await SeedScenarioAsync(
            "+972533000009",
            "+972533000010",
            "+972533000011",
            pendingCount: 1,
            includeCompleted: false,
            donationRemaining: 650m,
            recipientRemaining: 425m);
        var pendingId = Assert.Single(scenario.PendingRedemptionIds);
        using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"/api/admin/redemptions/{pendingId}/reject")
            .WithBearer(await Fixture.LoginAsync(scenario.AdminPhone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.ReadJsonAsync();
        Assert.Equal("REJECTED", json.GetProperty("status").GetString());
        Assert.Equal(scenario.AdminId, json.GetProperty("handledByAdminId").GetInt32());
        await Fixture.WithDbAsync(async db =>
        {
            var redemption = await db.Redemptions.AsNoTracking().SingleAsync(x => x.Id == pendingId);
            Assert.Equal("REJECTED", redemption.Status);
            Assert.Equal(scenario.AdminId, redemption.HandledByAdminId);
            Assert.Null(redemption.RedeemedAt);
            Assert.Equal(650m, await db.Donations.Where(x => x.Id == scenario.DonationId)
                .Select(x => x.RemainingAmount).SingleAsync());
            Assert.Equal(425m, await db.Recipients.Where(x => x.Id == scenario.RecipientId)
                .Select(x => x.RemainingEligibility).SingleAsync());
        });
    }

    [Fact]
    public async Task ApproveRedemption_AlreadyCompleted_ReturnsConflictAndPreservesBalances()
    {
        var scenario = await SeedScenarioAsync(
            "+972533000012",
            "+972533000013",
            "+972533000014",
            pendingCount: 0,
            includeCompleted: true,
            donationRemaining: 700m,
            recipientRemaining: 400m,
            completedAmount: 100m);
        using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"/api/admin/redemptions/{scenario.CompletedRedemptionId}/approve")
            .WithBearer(await Fixture.LoginAsync(scenario.AdminPhone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await Fixture.WithDbAsync(async db =>
        {
            Assert.Equal(700m, await db.Donations.Where(x => x.Id == scenario.DonationId)
                .Select(x => x.RemainingAmount).SingleAsync());
            Assert.Equal(400m, await db.Recipients.Where(x => x.Id == scenario.RecipientId)
                .Select(x => x.RemainingEligibility).SingleAsync());
        });
    }

    [Fact]
    public async Task ApproveRedemptions_ConcurrentRequestsCompeteForSameCap_OnlyOneCompletes()
    {
        var scenario = await SeedScenarioAsync(
            "+972533000015",
            "+972533000016",
            "+972533000017",
            pendingCount: 2,
            includeCompleted: false,
            donationRemaining: 500m,
            recipientRemaining: 500m,
            cap: 300m,
            pendingAmount: 300m);
        var token = await Fixture.LoginAsync(scenario.AdminPhone);
        using var firstRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"/api/admin/redemptions/{scenario.PendingRedemptionIds[0]}/approve")
            .WithBearer(token);
        using var secondRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"/api/admin/redemptions/{scenario.PendingRedemptionIds[1]}/approve")
            .WithBearer(token);

        var responses = await Task.WhenAll(
            Client.SendAsync(firstRequest),
            Client.SendAsync(secondRequest));
        using var firstResponse = responses[0];
        using var secondResponse = responses[1];

        Assert.Equal(
            new[] { HttpStatusCode.OK, HttpStatusCode.Conflict },
            responses.Select(x => x.StatusCode).Order().ToArray());
        await Fixture.WithDbAsync(async db =>
        {
            var rows = await db.Redemptions.AsNoTracking()
                .Where(x => scenario.PendingRedemptionIds.Contains(x.Id))
                .ToListAsync();
            Assert.Single(rows, x => x.Status == "COMPLETED");
            Assert.Single(rows, x => x.Status == "PENDING");
            Assert.Equal(300m, rows.Single(x => x.Status == "COMPLETED").Amount);
            Assert.Equal(200m, await db.Donations.Where(x => x.Id == scenario.DonationId)
                .Select(x => x.RemainingAmount).SingleAsync());
            Assert.Equal(200m, await db.Recipients.Where(x => x.Id == scenario.RecipientId)
                .Select(x => x.RemainingEligibility).SingleAsync());
        });
    }

    [Fact]
    public async Task GetRedemptions_DonorToken_ReturnsForbidden()
    {
        var donor = await Fixture.SeedUserAsync("DONOR", "+972533000018");
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/admin/redemptions")
            .WithBearer(await Fixture.LoginAsync(donor.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private async Task<AdminScenario> SeedScenarioAsync(
        string adminPhone,
        string donorPhone,
        string recipientPhone,
        int pendingCount,
        bool includeCompleted,
        decimal donationRemaining = 1_000m,
        decimal recipientRemaining = 1_000m,
        decimal cap = 500m,
        decimal completedAmount = 100m,
        decimal pendingAmount = 200m)
    {
        var population = await Fixture.SeedPopulationTypeAsync(name: $"Admin-{Guid.NewGuid():N}");
        var adminUser = await Fixture.SeedUserAsync("ADMIN", adminPhone);
        var donorUser = await Fixture.SeedUserAsync("DONOR", donorPhone);
        var recipientUser = await Fixture.SeedUserAsync(
            "RECIPIENT", recipientPhone, populationTypeId: population.Id, eligibility: 2_000m);

        return await Fixture.WithDbAsync(async db =>
        {
            var adminId = await db.Admins.Where(x => x.UserId == adminUser.Id)
                .Select(x => x.Id).SingleAsync();
            var donorId = await db.Donors.Where(x => x.UserId == donorUser.Id)
                .Select(x => x.Id).SingleAsync();
            var recipient = await db.Recipients.SingleAsync(x => x.UserId == recipientUser.Id);
            recipient.RemainingEligibility = recipientRemaining;
            recipient.EligibilityAmount = Math.Max(recipient.EligibilityAmount, recipientRemaining);
            var donation = new Donation
            {
                DonorId = donorId,
                PopulationTypeId = population.Id,
                TotalAmount = 1_000m,
                RemainingAmount = donationRemaining,
                MaxAmountPerRecipient = cap,
                Description = "Admin test donation",
                Status = "ACTIVE",
                CreatedAt = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(3)
            };
            db.Donations.Add(donation);
            await db.SaveChangesAsync();

            int? completedId = null;
            if (includeCompleted)
            {
                var completed = new Redemption
                {
                    DonationId = donation.Id,
                    RecipientId = recipient.Id,
                    Amount = completedAmount,
                    Status = "COMPLETED",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-5),
                    RedeemedAt = DateTime.UtcNow.AddMinutes(-4),
                    HandledByAdminId = adminId
                };
                db.Redemptions.Add(completed);
                await db.SaveChangesAsync();
                completedId = completed.Id;
            }

            var pending = Enumerable.Range(0, pendingCount)
                .Select(index => new Redemption
                {
                    DonationId = donation.Id,
                    RecipientId = recipient.Id,
                    Amount = pendingAmount,
                    Status = "PENDING",
                    CreatedAt = DateTime.UtcNow.AddSeconds(index)
                })
                .ToArray();
            db.Redemptions.AddRange(pending);
            await db.SaveChangesAsync();

            return new AdminScenario(
                adminPhone,
                adminId,
                donation.Id,
                recipient.Id,
                pending.Select(x => x.Id).ToArray(),
                completedId);
        });
    }

    private sealed record AdminScenario(
        string AdminPhone,
        int AdminId,
        int DonationId,
        int RecipientId,
        int[] PendingRedemptionIds,
        int? CompletedRedemptionId);
}
