using BackendService.ApiTests.Infrastructure;
using BackendService.Data;
using BackendService.Models;
using BackendService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BackendService.ApiTests;

public sealed class MatchingIntegrationTests(ApiTestFixture fixture)
    : IntegrationTestBase(fixture)
{
    [Theory]
    [InlineData(80, 70, 100, 0, 70)]
    [InlineData(40, 70, 100, 0, 40)]
    [InlineData(80, 70, 50, 20, 30)]
    [InlineData(80, 70, 50, 50, 0)]
    public async Task CalculateMatchingAmount_UsesMinimumAndCompletedCumulativeCap(
        decimal donationRemaining, decimal eligibility, decimal cap,
        decimal completed, decimal expected)
    {
        var scenario = await SeedScenarioAsync(donationRemaining, eligibility, cap);
        if (completed > 0)
            await SeedRedemptionAsync(scenario.DonationId, scenario.RecipientId, completed, "COMPLETED");

        await using var scope = Fixture.Factory.Services.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<DonationMatchingService>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var donation = await db.Donations.AsNoTracking().SingleAsync(x => x.Id == scenario.DonationId);
        var recipient = await db.Recipients.AsNoTracking().SingleAsync(x => x.Id == scenario.RecipientId);

        var actual = await service.CalculateMatchingAmountAsync(donation, recipient);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("PENDING", false)]
    [InlineData("REJECTED", false)]
    [InlineData("CANCELLED", true)]
    [InlineData("COMPLETED", true)]
    public async Task TryCreateMatch_ExistingStatus_FollowsCurrentBlockingRules(
        string existingStatus, bool expectedCreated)
    {
        var scenario = await SeedScenarioAsync(100m, 100m, 100m);
        await SeedRedemptionAsync(scenario.DonationId, scenario.RecipientId, 10m, existingStatus);

        var created = await InvokeMatchingAsync(service =>
            service.TryCreateMatchAsync(scenario.DonationId, scenario.RecipientId));

        Assert.Equal(expectedCreated, created);
        var count = await Fixture.WithDbAsync(db => db.Redemptions.CountAsync(x =>
            x.DonationId == scenario.DonationId &&
            x.RecipientId == scenario.RecipientId &&
            x.Status == "PENDING"));
        Assert.Equal(expectedCreated ? 1 : existingStatus == "PENDING" ? 1 : 0, count);
    }

    [Theory]
    [InlineData(false, true, true, 100, 100)]
    [InlineData(true, false, true, 100, 100)]
    [InlineData(true, true, false, 100, 100)]
    [InlineData(true, true, true, 0, 100)]
    [InlineData(true, true, true, 100, 0)]
    public async Task TryCreateMatch_IneligibleState_DoesNotCreateRedemption(
        bool donorActive, bool recipientActive, bool donationActive,
        decimal donationRemaining, decimal eligibility)
    {
        var scenario = await SeedScenarioAsync(
            donationRemaining, eligibility, 100m,
            donorActive, recipientActive,
            donationActive ? "ACTIVE" : "CANCELLED");

        var created = await InvokeMatchingAsync(service =>
            service.TryCreateMatchAsync(scenario.DonationId, scenario.RecipientId));

        Assert.False(created);
        Assert.Equal(0, await Fixture.WithDbAsync(db => db.Redemptions.CountAsync()));
    }

    [Fact]
    public async Task TryCreateMatch_DifferentPopulation_DoesNotCreateRedemption()
    {
        var scenario = await SeedScenarioAsync(100m, 100m, 100m);
        var otherPopulation = await Fixture.SeedPopulationTypeAsync();
        await Fixture.WithDbAsync(async db =>
        {
            var recipient = await db.Recipients.SingleAsync(x => x.Id == scenario.RecipientId);
            recipient.PopulationTypeId = otherPopulation.Id;
            await db.SaveChangesAsync();
        });

        Assert.False(await InvokeMatchingAsync(service =>
            service.TryCreateMatchAsync(scenario.DonationId, scenario.RecipientId)));
        Assert.Equal(0, await Fixture.WithDbAsync(db => db.Redemptions.CountAsync()));
    }

    [Fact]
    public async Task TryCreateMatch_TwoConcurrentCalls_CreateAtMostOnePendingRedemption()
    {
        var scenario = await SeedScenarioAsync(100m, 100m, 100m);
        var calls = Enumerable.Range(0, 2)
            .Select(_ => InvokeMatchingAsync(service =>
                service.TryCreateMatchAsync(scenario.DonationId, scenario.RecipientId)))
            .ToArray();

        var results = await Task.WhenAll(calls);

        Assert.Single(results, x => x);
        Assert.Equal(1, await Fixture.WithDbAsync(db => db.Redemptions.CountAsync(x =>
            x.DonationId == scenario.DonationId &&
            x.RecipientId == scenario.RecipientId &&
            x.Status == "PENDING")));
    }

    [Fact]
    public async Task DonationCreatedEvent_MatchesAllEligibleRecipientsEventuallyWithoutDuplicates()
    {
        var population = await Fixture.SeedPopulationTypeAsync();
        var donor = await Fixture.SeedUserAsync("DONOR", "+972530000001");
        await Fixture.SeedUserAsync(
            "RECIPIENT", "+972530000002", populationTypeId: population.Id, eligibility: 75m);
        await Fixture.SeedUserAsync(
            "RECIPIENT", "+972530000003", populationTypeId: population.Id, eligibility: 50m);
        var token = await Fixture.LoginAsync(donor.Phone);
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/donations")
        {
            Content = JsonContent.Create(new
            {
                populationTypeId = population.Id,
                totalAmount = 200m,
                maxAmountPerRecipient = 60m,
                description = "worker test",
                expirationDate = DateTime.UtcNow.AddDays(1)
            })
        }.WithBearer(token);

        using var response = await Client.SendAsync(request);
        var donationId = (await response.ReadJsonAsync()).GetProperty("donationId").GetInt32();

        var redemptions = await Eventually.UntilAsync(
            () => Fixture.WithDbAsync(db => db.Redemptions.AsNoTracking()
                .Where(x => x.DonationId == donationId).ToListAsync()),
            rows => rows.Count == 2);
        Assert.Equal(new[] { 50m, 60m }, redemptions.Select(x => x.Amount).Order().ToArray());
        Assert.All(redemptions, x => Assert.Equal("PENDING", x.Status));
        Assert.Equal(2, redemptions.Select(x => x.RecipientId).Distinct().Count());
    }

    [Fact]
    public async Task RecipientRegisteredEvent_MatchesAvailableDonationEventually()
    {
        var population = await Fixture.SeedPopulationTypeAsync(baseAmount: 90m);
        var donor = await Fixture.SeedUserAsync("DONOR", "+972530000004");
        var donorId = await Fixture.WithDbAsync(db =>
            db.Donors.Where(x => x.UserId == donor.Id).Select(x => x.Id).SingleAsync());
        var donationId = await Fixture.WithDbAsync(async db =>
        {
            var donation = new Donation
            {
                DonorId = donorId,
                PopulationTypeId = population.Id,
                TotalAmount = 100m,
                RemainingAmount = 100m,
                MaxAmountPerRecipient = 80m,
                Status = "ACTIVE",
                CreatedAt = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(1)
            };
            db.Donations.Add(donation);
            await db.SaveChangesAsync();
            return donation.Id;
        });

        using var response = await Client.PostAsJsonAsync("/api/Auth/register/recipient", new
        {
            name = "New recipient",
            phone = "+972530000005",
            password = "Passw0rd!",
            populationTypeId = population.Id,
            incomeAmount = 0,
            amountOfChildren = 0
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var redemption = await Eventually.UntilAsync(
            () => Fixture.WithDbAsync(db => db.Redemptions.AsNoTracking()
                .SingleOrDefaultAsync(x => x.DonationId == donationId)),
            row => row is not null);
        Assert.Equal(80m, redemption!.Amount);
        Assert.Equal("PENDING", redemption.Status);
    }

    private async Task<(int DonationId, int RecipientId)> SeedScenarioAsync(
        decimal donationRemaining, decimal eligibility, decimal cap,
        bool donorActive = true, bool recipientActive = true,
        string donationStatus = "ACTIVE")
    {
        var population = await Fixture.SeedPopulationTypeAsync();
        var donor = await Fixture.SeedUserAsync(
            "DONOR", $"+97254{Guid.NewGuid():N}"[..14], active: donorActive);
        var recipient = await Fixture.SeedUserAsync(
            "RECIPIENT", $"+97255{Guid.NewGuid():N}"[..14], active: recipientActive,
            populationTypeId: population.Id, eligibility: eligibility);
        return await Fixture.WithDbAsync(async db =>
        {
            var donorId = await db.Donors.Where(x => x.UserId == donor.Id).Select(x => x.Id).SingleAsync();
            var recipientId = await db.Recipients.Where(x => x.UserId == recipient.Id).Select(x => x.Id).SingleAsync();
            var donation = new Donation
            {
                DonorId = donorId,
                PopulationTypeId = population.Id,
                TotalAmount = Math.Max(100m, donationRemaining),
                RemainingAmount = donationRemaining,
                MaxAmountPerRecipient = cap,
                Status = donationStatus,
                CreatedAt = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(1)
            };
            db.Donations.Add(donation);
            await db.SaveChangesAsync();
            return (donation.Id, recipientId);
        });
    }

    private Task SeedRedemptionAsync(int donationId, int recipientId, decimal amount, string status) =>
        Fixture.WithDbAsync(async db =>
        {
            db.Redemptions.Add(new Redemption
            {
                DonationId = donationId,
                RecipientId = recipientId,
                Amount = amount,
                Status = status,
                CreatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        });

    private async Task<T> InvokeMatchingAsync<T>(Func<DonationMatchingService, Task<T>> action)
    {
        await using var scope = Fixture.Factory.Services.CreateAsyncScope();
        return await action(scope.ServiceProvider.GetRequiredService<DonationMatchingService>());
    }
}
