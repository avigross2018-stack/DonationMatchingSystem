using BackendService.ApiTests.Infrastructure;
using BackendService.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendService.ApiTests;

public sealed class DonationApiTests(ApiTestFixture fixture)
    : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task CreateDonation_ValidPayload_PersistsServerOwnedFieldsAndIgnoresDonorId()
    {
        var population = await Fixture.SeedPopulationTypeAsync();
        var owner = await Fixture.SeedUserAsync("DONOR", "+972520000001");
        var other = await Fixture.SeedUserAsync("DONOR", "+972520000002");
        var otherDonorId = await Fixture.WithDbAsync(db =>
            db.Donors.Where(x => x.UserId == other.Id).Select(x => x.Id).SingleAsync());
        var expires = DateTime.UtcNow.AddDays(2);
        var before = DateTime.UtcNow;
        var token = await Fixture.LoginAsync(owner.Phone);
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/donations")
        {
            Content = JsonContent.Create(new
            {
                populationTypeId = population.Id,
                totalAmount = 500.25m,
                maxAmountPerRecipient = 125.10m,
                description = "סיוע לחג",
                expirationDate = expires,
                donorId = otherDonorId,
                remainingAmount = 1,
                status = "CANCELLED"
            })
        }.WithBearer(token);

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.ReadJsonAsync();
        Assert.Equal("Donation created successfully", body.GetProperty("message").GetString());
        var id = body.GetProperty("donationId").GetInt32();
        Assert.True(id > 0);
        await Fixture.WithDbAsync(async db =>
        {
            var donation = await db.Donations.AsNoTracking().SingleAsync(x => x.Id == id);
            var ownerDonorId = await db.Donors.Where(x => x.UserId == owner.Id).Select(x => x.Id).SingleAsync();
            Assert.Equal(ownerDonorId, donation.DonorId);
            Assert.NotEqual(otherDonorId, donation.DonorId);
            Assert.Equal(population.Id, donation.PopulationTypeId);
            Assert.Equal(500.25m, donation.TotalAmount);
            Assert.Equal(500.25m, donation.RemainingAmount);
            Assert.Equal(125.10m, donation.MaxAmountPerRecipient);
            Assert.Equal("סיוע לחג", donation.Description);
            Assert.Equal("ACTIVE", donation.Status);
            Assert.InRange(donation.CreatedAt, before.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
            Assert.InRange(donation.ExpirationDate!.Value, expires.AddSeconds(-1), expires.AddSeconds(1));
        });
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(10, 0)]
    [InlineData(100000000, 10)]
    public async Task CreateDonation_InvalidAmount_ReturnsBadRequestWithoutDonation(
        decimal total, decimal cap)
    {
        var population = await Fixture.SeedPopulationTypeAsync();
        var donor = await Fixture.SeedUserAsync("DONOR", "+972520000003");
        var token = await Fixture.LoginAsync(donor.Phone);
        using var request = CreateAuthorizedJsonRequest(HttpMethod.Post, "/api/donations", token, new
        {
            populationTypeId = population.Id,
            totalAmount = total,
            maxAmountPerRecipient = cap,
            description = "test",
            expirationDate = DateTime.UtcNow.AddDays(1)
        });

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, await Fixture.WithDbAsync(db => db.Donations.CountAsync()));
        Assert.Equal(0, await Fixture.WithDbAsync(db => db.Redemptions.CountAsync()));
    }

    [Fact]
    public async Task CreateDonation_InactivePopulation_ReturnsProblemDetailsWithoutSideEffects()
    {
        var population = await Fixture.SeedPopulationTypeAsync(active: false);
        var donor = await Fixture.SeedUserAsync("DONOR", "+972520000004");
        using var request = CreateAuthorizedJsonRequest(
            HttpMethod.Post, "/api/donations", await Fixture.LoginAsync(donor.Phone),
            DonationPayload(population.Id));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(0, await Fixture.WithDbAsync(db => db.Donations.CountAsync()));
    }

    [Fact]
    public async Task GetMyDonations_ReturnsOnlyOwnedDtoFields()
    {
        var population = await Fixture.SeedPopulationTypeAsync();
        var owner = await Fixture.SeedUserAsync("DONOR", "+972520000005");
        var other = await Fixture.SeedUserAsync("DONOR", "+972520000006");
        var owned = await SeedDonationAsync(owner.Id, population.Id, 300m, 100m, "owned");
        await SeedDonationAsync(other.Id, population.Id, 999m, 200m, "other");
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/donations/show-donations")
            .WithBearer(await Fixture.LoginAsync(owner.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var array = await response.ReadJsonAsync();
        var item = Assert.Single(array.EnumerateArray());
        Assert.Equal(owned.Id, item.GetProperty("id").GetInt32());
        Assert.Equal("owned", item.GetProperty("description").GetString());
        var names = item.EnumerateObject().Select(x => x.Name).Order().ToArray();
        Assert.Equal(new[]
        {
            "createdAt", "description", "expirationDate", "id",
            "maxAmountPerRecipient", "populationTypeId", "remainingAmount", "status", "totalAmount"
        }, names);
        var text = item.GetRawText();
        Assert.DoesNotContain("password", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("recipientId", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("recipients", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("donor", text, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetMyDonations_DonorWithoutDonations_ReturnsEmptyArray()
    {
        var donor = await Fixture.SeedUserAsync("DONOR", "+972520000007");
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/donations/show-donations")
            .WithBearer(await Fixture.LoginAsync(donor.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty((await response.ReadJsonAsync()).EnumerateArray());
    }

    [Fact]
    public async Task GetMyDonations_MissingProfile_ReturnsNotFoundProblemDetails()
    {
        var user = await Fixture.SeedUserAsync(
            "DONOR", "+972520000008", createProfile: false);
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/donations/show-donations")
            .WithBearer(await Fixture.LoginAsync(user.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task UpdateDonation_OwnerAndValidPayload_PreservesRedeemedAmountAndImmutableFields()
    {
        var population = await Fixture.SeedPopulationTypeAsync();
        var owner = await Fixture.SeedUserAsync("DONOR", "+972520000009");
        var donation = await SeedDonationAsync(owner.Id, population.Id, 500m, 100m, "before", 350m);
        var originalCreatedAt = await Fixture.WithDbAsync(db => db.Donations
            .AsNoTracking()
            .Where(x => x.Id == donation.Id)
            .Select(x => x.CreatedAt)
            .SingleAsync());
        var originalDonorId = donation.DonorId;
        var expires = DateTime.UtcNow.AddDays(5);
        using var request = CreateAuthorizedJsonRequest(
            HttpMethod.Put, $"/api/donations/{donation.Id}", await Fixture.LoginAsync(owner.Phone), new
            {
                totalAmount = 600m,
                maxAmountPerRecipient = 150m,
                description = "after",
                expirationDate = expires,
                donorId = 999,
                populationTypeId = 999,
                status = "CANCELLED"
            });

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsByteArrayAsync());
        await Fixture.WithDbAsync(async db =>
        {
            var updated = await db.Donations.AsNoTracking().SingleAsync(x => x.Id == donation.Id);
            Assert.Equal(600m, updated.TotalAmount);
            Assert.Equal(450m, updated.RemainingAmount);
            Assert.Equal(150m, updated.MaxAmountPerRecipient);
            Assert.Equal("after", updated.Description);
            Assert.Equal(originalDonorId, updated.DonorId);
            Assert.Equal(population.Id, updated.PopulationTypeId);
            Assert.Equal(originalCreatedAt, updated.CreatedAt);
            Assert.Equal("ACTIVE", updated.Status);
        });
    }

    [Fact]
    public async Task UpdateDonation_TotalBelowRedeemed_ReturnsConflictAndLeavesDonationUnchanged()
    {
        var population = await Fixture.SeedPopulationTypeAsync();
        var owner = await Fixture.SeedUserAsync("DONOR", "+972520000010");
        var donation = await SeedDonationAsync(owner.Id, population.Id, 500m, 100m, "before", 300m);
        using var request = CreateAuthorizedJsonRequest(
            HttpMethod.Put, $"/api/donations/{donation.Id}", await Fixture.LoginAsync(owner.Phone), new
            {
                totalAmount = 199m,
                maxAmountPerRecipient = 90m,
                description = "must not persist",
                expirationDate = DateTime.UtcNow.AddDays(2)
            });

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await Fixture.WithDbAsync(async db =>
        {
            var unchanged = await db.Donations.AsNoTracking().SingleAsync(x => x.Id == donation.Id);
            Assert.Equal(500m, unchanged.TotalAmount);
            Assert.Equal(300m, unchanged.RemainingAmount);
            Assert.Equal("before", unchanged.Description);
        });
    }

    [Fact]
    public async Task UpdateDonation_OtherDonor_ReturnsForbiddenAndLeavesDonationUnchanged()
    {
        var population = await Fixture.SeedPopulationTypeAsync();
        var owner = await Fixture.SeedUserAsync("DONOR", "+972520000011");
        var attacker = await Fixture.SeedUserAsync("DONOR", "+972520000012");
        var donation = await SeedDonationAsync(owner.Id, population.Id, 500m, 100m, "before");
        using var request = CreateAuthorizedJsonRequest(
            HttpMethod.Put, $"/api/donations/{donation.Id}", await Fixture.LoginAsync(attacker.Phone),
            new { totalAmount = 600m, maxAmountPerRecipient = 200m, description = "hijack", expirationDate = DateTime.UtcNow.AddDays(2) });

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("before", await Fixture.WithDbAsync(db =>
            db.Donations.Where(x => x.Id == donation.Id).Select(x => x.Description).SingleAsync()));
    }

    [Fact]
    public async Task CancelDonation_ActiveOwnedDonation_CancelsOnlyPendingRedemptions()
    {
        var population = await Fixture.SeedPopulationTypeAsync();
        var owner = await Fixture.SeedUserAsync("DONOR", "+972520000013");
        var recipientUser = await Fixture.SeedUserAsync(
            "RECIPIENT", "+972520000014", populationTypeId: population.Id);
        var recipientId = await Fixture.WithDbAsync(db =>
            db.Recipients.Where(x => x.UserId == recipientUser.Id).Select(x => x.Id).SingleAsync());
        var donation = await SeedDonationAsync(owner.Id, population.Id, 500m, 100m, "cancel");
        await Fixture.WithDbAsync(async db =>
        {
            foreach (var status in new[] { "PENDING", "COMPLETED", "REJECTED", "CANCELLED" })
                db.Redemptions.Add(new Redemption
                {
                    DonationId = donation.Id,
                    RecipientId = recipientId,
                    Amount = 10m,
                    Status = status,
                    CreatedAt = DateTime.UtcNow
                });
            await db.SaveChangesAsync();
        });
        using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/donations/{donation.Id}/cancel")
            .WithBearer(await Fixture.LoginAsync(owner.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        await Fixture.WithDbAsync(async db =>
        {
            Assert.Equal("CANCELLED", (await db.Donations.FindAsync(donation.Id))!.Status);
            var statuses = await db.Redemptions.AsNoTracking()
                .Where(x => x.DonationId == donation.Id).Select(x => x.Status).ToListAsync();
            Assert.Equal(2, statuses.Count(x => x == "CANCELLED"));
            Assert.Contains("COMPLETED", statuses);
            Assert.Contains("REJECTED", statuses);
        });
    }

    [Fact]
    public async Task CancelDonation_Twice_SecondCallReturnsConflict()
    {
        var population = await Fixture.SeedPopulationTypeAsync();
        var owner = await Fixture.SeedUserAsync("DONOR", "+972520000015");
        var donation = await SeedDonationAsync(owner.Id, population.Id, 500m, 100m, "cancel");
        var token = await Fixture.LoginAsync(owner.Phone);

        using (var firstRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/donations/{donation.Id}/cancel").WithBearer(token))
        using (var first = await Client.SendAsync(firstRequest))
            Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
        using var secondRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/donations/{donation.Id}/cancel").WithBearer(token);
        using var second = await Client.SendAsync(secondRequest);

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
        var body = await second.ReadJsonAsync();
        Assert.True(body.TryGetProperty("message", out _));
    }

    private async Task<Donation> SeedDonationAsync(
        int donorUserId, int populationTypeId, decimal total, decimal cap,
        string description, decimal? remaining = null)
    {
        return await Fixture.WithDbAsync(async db =>
        {
            var donorId = await db.Donors.Where(x => x.UserId == donorUserId).Select(x => x.Id).SingleAsync();
            var donation = new Donation
            {
                DonorId = donorId,
                PopulationTypeId = populationTypeId,
                TotalAmount = total,
                RemainingAmount = remaining ?? total,
                MaxAmountPerRecipient = cap,
                Description = description,
                Status = "ACTIVE",
                CreatedAt = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(3)
            };
            db.Donations.Add(donation);
            await db.SaveChangesAsync();
            return donation;
        });
    }

    private static object DonationPayload(int populationTypeId) => new
    {
        populationTypeId,
        totalAmount = 100m,
        maxAmountPerRecipient = 50m,
        description = "test",
        expirationDate = DateTime.UtcNow.AddDays(1)
    };

    private static HttpRequestMessage CreateAuthorizedJsonRequest(
        HttpMethod method, string path, string token, object body) =>
        new HttpRequestMessage(method, path) { Content = JsonContent.Create(body) }.WithBearer(token);
}
