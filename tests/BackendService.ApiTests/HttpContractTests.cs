using BackendService.ApiTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BackendService.ApiTests;

public sealed class HttpContractTests(ApiTestFixture fixture)
    : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task ModelValidation_InvalidFields_ReturnsValidationProblemDetailsWithCamelCaseErrors()
    {
        using var response = await Client.PostAsJsonAsync("/api/Auth/register/donor", new
        {
            name = new string('x', 51),
            phone = "invalid",
            password = "short"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var body = await response.ReadJsonAsync();
        Assert.Equal(400, body.GetProperty("status").GetInt32());
        Assert.True(body.GetProperty("errors").EnumerateObject().Any());
        Assert.Equal(0, await Fixture.WithDbAsync(db => db.Users.CountAsync()));
    }

    [Fact]
    public async Task GlobalException_Conflict_HasStableProblemDetailsEnvelope()
    {
        await Fixture.SeedUserAsync("DONOR", "+972560000001");

        using var response = await Client.PostAsJsonAsync("/api/Auth/register/donor", new
        {
            name = "Duplicate",
            phone = "+972560000001",
            password = "Passw0rd!"
        });

        var body = await response.ReadJsonAsync();
        Assert.Equal(409, body.GetProperty("status").GetInt32());
        Assert.Equal("Conflict", body.GetProperty("title").GetString());
        Assert.False(string.IsNullOrWhiteSpace(body.GetProperty("detail").GetString()));
        Assert.Equal("/api/Auth/register/donor", body.GetProperty("instance").GetString());
        Assert.False(string.IsNullOrWhiteSpace(body.GetProperty("traceId").GetString()));
        var raw = body.GetRawText();
        Assert.DoesNotContain("stack", raw, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("connection", raw, StringComparison.OrdinalIgnoreCase);
    }
}
