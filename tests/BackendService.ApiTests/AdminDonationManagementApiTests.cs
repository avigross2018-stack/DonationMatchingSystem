using BackendService.ApiTests.Infrastructure;
using BackendService.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendService.ApiTests;

public sealed class AdminDonationManagementApiTests(
    ApiTestFixture fixture) : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task CancelDonation_ActiveDonation_CancelsPendingOnly()
    {
        var population = await Fixture.SeedPopulationTypeAsync();
        var admin = await Fixture.SeedUserAsync(
            "ADMIN", "+972533200001");
        var donor = await Fixture.SeedUserAsync(
            "DONOR", "+972533200002");
        var recipientUser = await Fixture.SeedUserAsync(
            "RECIPIENT",
            "+972533200003",
            populationTypeId: population.Id);

        var recipientId = await Fixture.WithDbAsync(db =>
            db.Recipients
                .Where(r => r.UserId == recipientUser.Id)
                .Select(r => r.Id)
                .SingleAsync());

        var donationId = await CreateDonationAsync(
            donor.Id,
            population.Id);

        await Fixture.WithDbAsync(async db =>
        {
            foreach (var status in new[]
                     {
                         "PENDING",
                         "COMPLETED",
                         "REJECTED"
                     })
            {
                db.Redemptions.Add(new Redemption
                {
                    DonationId = donationId,
                    RecipientId = recipientId,
                    Amount = 25m,
                    Status = status,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await db.SaveChangesAsync();
        });

        using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"/api/admin/donations/{donationId}/cancel")
            .WithBearer(await Fixture.LoginAsync(admin.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        await Fixture.WithDbAsync(async db =>
        {
            var donation = await db.Donations
                .AsNoTracking()
                .SingleAsync(d => d.Id == donationId);

            Assert.Equal("CANCELLED", donation.Status);
            Assert.Equal(500m, donation.TotalAmount);
            Assert.Equal(400m, donation.RemainingAmount);

            var redemptions = await db.Redemptions
                .AsNoTracking()
                .Where(r => r.DonationId == donationId)
                .ToListAsync();

            Assert.Single(redemptions, r => r.Status == "CANCELLED");
            Assert.Single(redemptions, r => r.Status == "COMPLETED");
            Assert.Single(redemptions, r => r.Status == "REJECTED");
        });
    }

    [Fact]
    public async Task CancelDonation_AlreadyCancelled_ReturnsConflict()
    {
        var population = await Fixture.SeedPopulationTypeAsync();
        var admin = await Fixture.SeedUserAsync(
            "ADMIN", "+972533200011");
        var donor = await Fixture.SeedUserAsync(
            "DONOR", "+972533200012");

        var donationId = await CreateDonationAsync(
            donor.Id,
            population.Id);

        using var firstRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"/api/admin/donations/{donationId}/cancel")
            .WithBearer(await Fixture.LoginAsync(admin.Phone));

        using var firstResponse = await Client.SendAsync(firstRequest);

        Assert.Equal(
            HttpStatusCode.NoContent,
            firstResponse.StatusCode);

        using var secondRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"/api/admin/donations/{donationId}/cancel")
            .WithBearer(await Fixture.LoginAsync(admin.Phone));

        using var secondResponse = await Client.SendAsync(secondRequest);

        Assert.Equal(
            HttpStatusCode.Conflict,
            secondResponse.StatusCode);

        await Fixture.WithDbAsync(async db =>
        {
            var donation = await db.Donations
                .AsNoTracking()
                .SingleAsync(d => d.Id == donationId);

            Assert.Equal("CANCELLED", donation.Status);
        });
    }

    [Fact]
    public async Task CancelDonation_DonorToken_ReturnsForbidden()
    {
        var population = await Fixture.SeedPopulationTypeAsync();
        var admin = await Fixture.SeedUserAsync(
            "ADMIN", "+972533200021");
        var donor = await Fixture.SeedUserAsync(
            "DONOR", "+972533200022");

        var donationId = await CreateDonationAsync(
            donor.Id,
            population.Id);

        using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"/api/admin/donations/{donationId}/cancel")
            .WithBearer(await Fixture.LoginAsync(donor.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        await Fixture.WithDbAsync(async db =>
        {
            var donation = await db.Donations
                .AsNoTracking()
                .SingleAsync(d => d.Id == donationId);

            Assert.Equal("ACTIVE", donation.Status);
        });
    }

    private async Task<int> CreateDonationAsync(
        int donorUserId,
        int populationTypeId)
    {
        return await Fixture.WithDbAsync(async db =>
        {
            var donorId = await db.Donors
                .Where(d => d.UserId == donorUserId)
                .Select(d => d.Id)
                .SingleAsync();

            var donation = new Donation
            {
                DonorId = donorId,
                PopulationTypeId = populationTypeId,
                TotalAmount = 500m,
                RemainingAmount = 400m,
                MaxAmountPerRecipient = 100m,
                Description = "Admin cancellation test",
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
