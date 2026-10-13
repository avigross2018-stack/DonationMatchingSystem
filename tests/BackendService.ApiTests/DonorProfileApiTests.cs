using BackendService.ApiTests.Infrastructure;
using BackendService.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendService.ApiTests;

public sealed class DonorProfileApiTests(ApiTestFixture fixture)
    : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task GetMyProfile_ActiveDonor_ReturnsOnlyOwnProfileFields()
    {
        var user = await Fixture.SeedUserAsync("DONOR", "+972531000001");
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/donors/me")
            .WithBearer(await Fixture.LoginAsync(user.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.ReadJsonAsync();
        var donorId = await Fixture.WithDbAsync(db => db.Donors
            .Where(x => x.UserId == user.Id)
            .Select(x => x.Id)
            .SingleAsync());
        Assert.Equal(donorId, json.GetProperty("id").GetInt32());
        Assert.Equal(user.Name, json.GetProperty("name").GetString());
        Assert.Equal(user.Phone, json.GetProperty("phone").GetString());
        Assert.True(json.TryGetProperty("createdAt", out _));
        Assert.Equal(
            new[] { "createdAt", "id", "name", "phone" },
            json.EnumerateObject().Select(x => x.Name).Order().ToArray());
    }

    [Fact]
    public async Task UpdateMyProfile_ValidPayload_TrimsAndPersistsNameAndPhone()
    {
        var user = await Fixture.SeedUserAsync("DONOR", "+972531000002");
        using var request = CreateUpdateRequest(
            await Fixture.LoginAsync(user.Phone),
            "  Updated donor  ",
            "  +972531000099  ");

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsByteArrayAsync());
        var persisted = await Fixture.WithDbAsync(db => db.Users
            .AsNoTracking()
            .SingleAsync(x => x.Id == user.Id));
        Assert.Equal("Updated donor", persisted.Name);
        Assert.Equal("+972531000099", persisted.Phone);
    }

    [Fact]
    public async Task UpdateMyProfile_DuplicatePhone_ReturnsConflictAndLeavesProfileUnchanged()
    {
        var donor = await Fixture.SeedUserAsync("DONOR", "+972531000003");
        var other = await Fixture.SeedUserAsync("DONOR", "+972531000004");
        using var request = CreateUpdateRequest(
            await Fixture.LoginAsync(donor.Phone),
            "Must not persist",
            other.Phone);

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var persisted = await Fixture.WithDbAsync(db => db.Users
            .AsNoTracking()
            .SingleAsync(x => x.Id == donor.Id));
        Assert.Equal(donor.Name, persisted.Name);
        Assert.Equal(donor.Phone, persisted.Phone);
    }

    [Fact]
    public async Task DeactivateMyAccount_ActiveDonor_CancelsActiveDonationsAndOnlyPendingRedemptions()
    {
        var population = await Fixture.SeedPopulationTypeAsync();
        var donor = await Fixture.SeedUserAsync("DONOR", "+972531000005");
        var recipient = await Fixture.SeedUserAsync(
            "RECIPIENT", "+972531000006", populationTypeId: population.Id);
        var seeded = await SeedDeactivationGraphAsync(
            donor.Id,
            recipient.Id,
            population.Id);
        var token = await Fixture.LoginAsync(donor.Phone);
        using var request = new HttpRequestMessage(HttpMethod.Delete, "/api/donors/me")
            .WithBearer(token);

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        await Fixture.WithDbAsync(async db =>
        {
            Assert.False(await db.Users.Where(x => x.Id == donor.Id).Select(x => x.IsActive).SingleAsync());
            Assert.Equal("CANCELLED", await db.Donations.Where(x => x.Id == seeded.ActiveDonationId)
                .Select(x => x.Status).SingleAsync());
            Assert.Equal("CANCELLED", await db.Donations.Where(x => x.Id == seeded.AlreadyCancelledDonationId)
                .Select(x => x.Status).SingleAsync());
            var statuses = await db.Redemptions.AsNoTracking()
                .Where(x => x.DonationId == seeded.ActiveDonationId)
                .OrderBy(x => x.Id)
                .Select(x => x.Status)
                .ToListAsync();
            Assert.Equal(new[] { "CANCELLED", "COMPLETED", "REJECTED", "CANCELLED" }, statuses);
        });

        using var afterDeactivation = new HttpRequestMessage(HttpMethod.Get, "/api/donors/me")
            .WithBearer(token);
        using var rejected = await Client.SendAsync(afterDeactivation);
        Assert.Equal(HttpStatusCode.Unauthorized, rejected.StatusCode);
    }

    [Fact]
    public async Task GetMyProfile_RecipientToken_ReturnsForbidden()
    {
        var population = await Fixture.SeedPopulationTypeAsync();
        var recipient = await Fixture.SeedUserAsync(
            "RECIPIENT", "+972531000007", populationTypeId: population.Id);
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/donors/me")
            .WithBearer(await Fixture.LoginAsync(recipient.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private async Task<(int ActiveDonationId, int AlreadyCancelledDonationId)>
        SeedDeactivationGraphAsync(int donorUserId, int recipientUserId, int populationTypeId)
    {
        return await Fixture.WithDbAsync(async db =>
        {
            var donorId = await db.Donors.Where(x => x.UserId == donorUserId)
                .Select(x => x.Id).SingleAsync();
            var recipientId = await db.Recipients.Where(x => x.UserId == recipientUserId)
                .Select(x => x.Id).SingleAsync();
            var active = NewDonation(donorId, populationTypeId, "ACTIVE");
            var cancelled = NewDonation(donorId, populationTypeId, "CANCELLED");
            db.Donations.AddRange(active, cancelled);
            await db.SaveChangesAsync();

            foreach (var status in new[] { "PENDING", "COMPLETED", "REJECTED", "CANCELLED" })
            {
                db.Redemptions.Add(new Redemption
                {
                    DonationId = active.Id,
                    RecipientId = recipientId,
                    Amount = 25m,
                    Status = status,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await db.SaveChangesAsync();
            return (active.Id, cancelled.Id);
        });
    }

    private static Donation NewDonation(int donorId, int populationTypeId, string status) => new()
    {
        DonorId = donorId,
        PopulationTypeId = populationTypeId,
        TotalAmount = 500m,
        RemainingAmount = 500m,
        MaxAmountPerRecipient = 100m,
        Description = status,
        Status = status,
        CreatedAt = DateTime.UtcNow,
        ExpirationDate = DateTime.UtcNow.AddDays(2)
    };

    private static HttpRequestMessage CreateUpdateRequest(
        string token,
        string name,
        string phone) =>
        new HttpRequestMessage(HttpMethod.Put, "/api/donors/me")
        {
            Content = JsonContent.Create(new { name, phone })
        }.WithBearer(token);
}
