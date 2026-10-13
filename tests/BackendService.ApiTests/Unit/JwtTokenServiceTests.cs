using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BackendService.Models;
using BackendService.Services;
using Microsoft.Extensions.Configuration;

namespace BackendService.ApiTests.Unit;

public sealed class JwtTokenServiceTests
{
    [Fact]
    public void GenerateToken_ValidConfiguration_IncludesSubjectRoleAndUniqueJti()
    {
        var service = CreateService();
        var user = new User { Id = 42, UserType = "DONOR" };

        var first = new JwtSecurityTokenHandler().ReadJwtToken(service.GenerateToken(user));
        var second = new JwtSecurityTokenHandler().ReadJwtToken(service.GenerateToken(user));

        Assert.Equal("42", first.Subject);
        Assert.Equal("DONOR", first.Claims.Single(x => x.Type == ClaimTypes.Role).Value);
        Assert.NotEqual(first.Id, second.Id);
    }

    [Theory]
    [InlineData("JWT:KEY")]
    [InlineData("JWT:ISSUER")]
    [InlineData("JWT:AUDIENCE")]
    public void GenerateToken_MissingRequiredConfiguration_Throws(string missingKey)
    {
        var values = ValidConfiguration();
        values.Remove(missingKey);
        var service = new JwtTokenService(new ConfigurationBuilder()
            .AddInMemoryCollection(values).Build());

        Assert.Throws<InvalidOperationException>(() =>
            service.GenerateToken(new User { Id = 1, UserType = "DONOR" }));
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    public void GenerateToken_NonPositiveExpiration_Throws(string minutes)
    {
        var values = ValidConfiguration();
        values["JWT:EXPIRATION_MINUTES"] = minutes;
        var service = new JwtTokenService(new ConfigurationBuilder()
            .AddInMemoryCollection(values).Build());

        Assert.Throws<InvalidOperationException>(() =>
            service.GenerateToken(new User { Id = 1, UserType = "DONOR" }));
    }

    private static JwtTokenService CreateService() => new(
        new ConfigurationBuilder().AddInMemoryCollection(ValidConfiguration()).Build());

    private static Dictionary<string, string?> ValidConfiguration() => new()
    {
        ["JWT:KEY"] = "unit-tests-only-signing-key-at-least-32-bytes-long",
        ["JWT:ISSUER"] = "unit-tests",
        ["JWT:AUDIENCE"] = "unit-tests-client",
        ["JWT:EXPIRATION_MINUTES"] = "5"
    };
}
