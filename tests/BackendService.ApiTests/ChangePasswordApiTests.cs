using BackendService.ApiTests.Infrastructure;
using BackendService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BackendService.ApiTests;

public sealed class ChangePasswordApiTests(ApiTestFixture fixture)
    : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task ChangePassword_ValidCurrentPassword_ReplacesHashAndChangesLoginCredentials()
    {
        const string currentPassword = "Passw0rd!";
        const string newPassword = "NewPassw0rd!";
        var user = await Fixture.SeedUserAsync(
            "DONOR", "+972530000001", currentPassword);
        var originalHash = user.PasswordHash;
        var token = await Fixture.LoginAsync(user.Phone, currentPassword);
        using var request = CreateRequest(token, currentPassword, newPassword);

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsByteArrayAsync());

        var persisted = await Fixture.WithDbAsync(db => db.Users
            .AsNoTracking()
            .SingleAsync(x => x.Id == user.Id));
        Assert.NotEqual(originalHash, persisted.PasswordHash);
        Assert.Equal(
            PasswordVerificationResult.Success,
            new PasswordHasher<User>().VerifyHashedPassword(
                persisted,
                persisted.PasswordHash,
                newPassword));

        using var oldLogin = await Client.PostAsJsonAsync("/api/Auth/login", new
        {
            phone = user.Phone,
            password = currentPassword
        });
        Assert.Equal(HttpStatusCode.Unauthorized, oldLogin.StatusCode);

        using var newLogin = await Client.PostAsJsonAsync("/api/Auth/login", new
        {
            phone = user.Phone,
            password = newPassword
        });
        Assert.Equal(HttpStatusCode.OK, newLogin.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_IncorrectCurrentPassword_ReturnsBadRequestAndPreservesHash()
    {
        var user = await Fixture.SeedUserAsync("RECIPIENT", "+972530000002",
            populationTypeId: (await Fixture.SeedPopulationTypeAsync()).Id);
        var originalHash = user.PasswordHash;
        using var request = CreateRequest(
            await Fixture.LoginAsync(user.Phone),
            "Incorrect1!",
            "NewPassw0rd!");

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(originalHash, await Fixture.WithDbAsync(db => db.Users
            .Where(x => x.Id == user.Id)
            .Select(x => x.PasswordHash)
            .SingleAsync()));
    }

    [Fact]
    public async Task ChangePassword_NewPasswordMatchesCurrent_ReturnsBadRequestAndPreservesHash()
    {
        const string password = "Passw0rd!";
        var user = await Fixture.SeedUserAsync("ADMIN", "+972530000003", password);
        var originalHash = user.PasswordHash;
        using var request = CreateRequest(
            await Fixture.LoginAsync(user.Phone, password),
            password,
            password);

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(originalHash, await Fixture.WithDbAsync(db => db.Users
            .Where(x => x.Id == user.Id)
            .Select(x => x.PasswordHash)
            .SingleAsync()));
    }

    [Fact]
    public async Task ChangePassword_ShortNewPassword_ReturnsValidationProblemWithoutChangingHash()
    {
        var user = await Fixture.SeedUserAsync("DONOR", "+972530000004");
        var originalHash = user.PasswordHash;
        using var request = CreateRequest(
            await Fixture.LoginAsync(user.Phone),
            "Passw0rd!",
            "short");

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.ReadJsonAsync();
        Assert.True(problem.GetProperty("errors").TryGetProperty("NewPassword", out _));
        Assert.Equal(originalHash, await Fixture.WithDbAsync(db => db.Users
            .Where(x => x.Id == user.Id)
            .Select(x => x.PasswordHash)
            .SingleAsync()));
    }

    [Fact]
    public async Task ChangePassword_WithoutToken_ReturnsUnauthorized()
    {
        using var response = await Client.PutAsJsonAsync("/api/Auth/change-password", new
        {
            currentPassword = "Passw0rd!",
            newPassword = "NewPassw0rd!"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static HttpRequestMessage CreateRequest(
        string token,
        string currentPassword,
        string newPassword) =>
        new HttpRequestMessage(HttpMethod.Put, "/api/Auth/change-password")
        {
            Content = JsonContent.Create(new { currentPassword, newPassword })
        }.WithBearer(token);
}
