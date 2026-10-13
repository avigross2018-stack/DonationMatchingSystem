using BackendService.ApiTests.Infrastructure;
using BackendService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BackendService.ApiTests;

public sealed class AuthRegistrationTests(ApiTestFixture fixture)
    : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task RegisterDonor_ValidPayload_ReturnsCreatedAndPersistsOnlyDonorProfile()
    {
        var before = DateTime.UtcNow;

        using var response = await Client.PostAsJsonAsync("/api/Auth/register/donor", new
        {
            name = "תורמת בדיקה",
            phone = "+972501234567",
            password = "VerySafe1!",
            userType = "ADMIN",
            isActive = false,
            id = 999,
            passwordHash = "plain-text"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var json = await response.ReadJsonAsync();
        Assert.Equal("Donor registered successfully", json.GetProperty("message").GetString());
        var userId = json.GetProperty("userId").GetInt32();
        Assert.True(userId > 0);

        await Fixture.WithDbAsync(async db =>
        {
            var user = await db.Users.AsNoTracking().SingleAsync(x => x.Id == userId);
            Assert.Equal("DONOR", user.UserType);
            Assert.True(user.IsActive);
            Assert.InRange(user.CreatedAt, before.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
            Assert.NotEqual("VerySafe1!", user.PasswordHash);
            Assert.NotEqual("plain-text", user.PasswordHash);
            Assert.Equal(
                PasswordVerificationResult.Success,
                new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, "VerySafe1!"));
            Assert.True(await db.Donors.AnyAsync(x => x.UserId == userId));
            Assert.False(await db.Recipients.AnyAsync(x => x.UserId == userId));
            Assert.False(await db.Admins.AnyAsync(x => x.UserId == userId));
        });

        var responseText = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("VerySafe1!", responseText);
        Assert.DoesNotContain("passwordHash", responseText, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("{\"phone\":\"+972501111111\",\"password\":\"12345678\"}")]
    [InlineData("{\"name\":\"Name\",\"phone\":\"bad phone\",\"password\":\"12345678\"}")]
    [InlineData("{\"name\":\"Name\",\"phone\":\"+972501111111\",\"password\":\"short\"}")]
    [InlineData("{not-json")]
    public async Task RegisterDonor_InvalidPayload_ReturnsBadRequestWithoutSideEffects(string json)
    {
        using var response = await Client.PostAsync(
            "/api/Auth/register/donor",
            new StringContent(json, System.Text.Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, await Fixture.WithDbAsync(db => db.Users.CountAsync()));
        Assert.Equal(0, await Fixture.WithDbAsync(db => db.Donors.CountAsync()));
    }

    [Fact]
    public async Task RegisterDonor_DuplicatePhone_ReturnsConflictProblemDetailsWithoutExtraRows()
    {
        await Fixture.SeedUserAsync("DONOR", "+972502222222");

        using var response = await Client.PostAsJsonAsync("/api/Auth/register/donor", new
        {
            name = "Duplicate",
            phone = "+972502222222",
            password = "Passw0rd!"
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problem = await response.ReadJsonAsync();
        Assert.Equal(409, problem.GetProperty("status").GetInt32());
        Assert.Equal("Conflict", problem.GetProperty("title").GetString());
        Assert.Equal("/api/Auth/register/donor", problem.GetProperty("instance").GetString());
        Assert.True(problem.TryGetProperty("traceId", out _));
        Assert.Equal(1, await Fixture.WithDbAsync(db => db.Users.CountAsync()));
        Assert.Equal(1, await Fixture.WithDbAsync(db => db.Donors.CountAsync()));
    }

    [Fact]
    public async Task RegisterRecipient_ValidPayload_CalculatesEligibilityAndPersistsProfile()
    {
        var population = await Fixture.SeedPopulationTypeAsync(baseAmount: 1_000.01m);

        using var response = await Client.PostAsJsonAsync("/api/Auth/register/recipient", new
        {
            name = "Recipient",
            phone = "+972503333333",
            password = "Passw0rd!",
            populationTypeId = population.Id,
            incomeAmount = 333.33m,
            amountOfChildren = 2,
            requestReason = "Need assistance"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var userId = (await response.ReadJsonAsync()).GetProperty("userId").GetInt32();
        await Fixture.WithDbAsync(async db =>
        {
            var recipient = await db.Recipients.AsNoTracking().SingleAsync(x => x.UserId == userId);
            Assert.Equal(population.Id, recipient.PopulationTypeId);
            Assert.Equal(333.33m, recipient.IncomeAmount);
            Assert.Equal(2, recipient.AmountOfChildren);
            Assert.Equal("Need assistance", recipient.RequestReason);
            Assert.Equal(1_933.34m, recipient.EligibilityAmount);
            Assert.Equal(recipient.EligibilityAmount, recipient.RemainingEligibility);
            Assert.Equal("RECIPIENT", (await db.Users.FindAsync(userId))!.UserType);
        });
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(10.123, 0)]
    [InlineData(10, -1)]
    public async Task RegisterRecipient_InvalidEligibilityInput_ReturnsBadRequestWithoutOrphanUser(
        decimal income, int children)
    {
        var population = await Fixture.SeedPopulationTypeAsync();

        using var response = await Client.PostAsJsonAsync("/api/Auth/register/recipient", new
        {
            name = "Recipient",
            phone = "+972504444444",
            password = "Passw0rd!",
            populationTypeId = population.Id,
            incomeAmount = income,
            amountOfChildren = children
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, await Fixture.WithDbAsync(db => db.Users.CountAsync()));
        Assert.Equal(0, await Fixture.WithDbAsync(db => db.Recipients.CountAsync()));
    }

    [Fact]
    public async Task RegisterRecipient_InactivePopulation_ReturnsProblemDetailsWithoutOrphanUser()
    {
        var population = await Fixture.SeedPopulationTypeAsync(active: false);

        using var response = await Client.PostAsJsonAsync("/api/Auth/register/recipient", new
        {
            name = "Recipient",
            phone = "+972505555555",
            password = "Passw0rd!",
            populationTypeId = population.Id,
            incomeAmount = 0,
            amountOfChildren = 0
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(0, await Fixture.WithDbAsync(db => db.Users.CountAsync()));
    }
}
