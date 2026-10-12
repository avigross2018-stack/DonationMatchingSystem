using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackendService.ApiTests.Infrastructure;
using Microsoft.IdentityModel.Tokens;

namespace BackendService.ApiTests;

public sealed class LoginAndAuthorizationTests(ApiTestFixture fixture)
    : IntegrationTestBase(fixture)
{
    [Theory]
    [InlineData("DONOR")]
    [InlineData("ADMIN")]
    [InlineData("RECIPIENT")]
    public async Task Login_ValidCredentials_ReturnsCryptographicallyValidJwt(string role)
    {
        int? populationId = null;
        if (role == "RECIPIENT")
            populationId = (await Fixture.SeedPopulationTypeAsync()).Id;
        var user = await Fixture.SeedUserAsync(
            role, $"+97251{Guid.NewGuid():N}"[..14], populationTypeId: populationId);

        using var response = await Client.PostAsJsonAsync("/api/Auth/login", new
        {
            phone = user.Phone,
            password = "Passw0rd!"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.ReadJsonAsync();
        Assert.Equal("Bearer", body.GetProperty("tokenType").GetString());
        var token = body.GetProperty("accessToken").GetString();
        Assert.False(string.IsNullOrWhiteSpace(token));

        var validation = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = TestSettings.JwtIssuer,
            ValidateAudience = true,
            ValidAudience = TestSettings.JwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSettings.JwtKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            NameClaimType = JwtRegisteredClaimNames.Sub,
            RoleClaimType = ClaimTypes.Role
        };
        var principal = new JwtSecurityTokenHandler().ValidateToken(token, validation, out var validated);
        Assert.Equal(user.Id.ToString(), principal.FindFirstValue(JwtRegisteredClaimNames.Sub));
        Assert.Equal(role, principal.FindFirstValue(ClaimTypes.Role));
        Assert.False(string.IsNullOrWhiteSpace(principal.FindFirstValue(JwtRegisteredClaimNames.Jti)));
        Assert.Equal(SecurityAlgorithms.HmacSha256, ((JwtSecurityToken)validated).Header.Alg);
    }

    [Theory]
    [InlineData("missing", "Passw0rd!", true)]
    [InlineData("existing", "wrong-password", true)]
    [InlineData("existing", "Passw0rd!", false)]
    public async Task Login_InvalidCredentialsOrInactiveUser_ReturnsSameSafeUnauthorizedShape(
        string phoneKind, string password, bool active)
    {
        const string realPhone = "+972506666666";
        await Fixture.SeedUserAsync("DONOR", realPhone, active: active);

        using var response = await Client.PostAsJsonAsync("/api/Auth/login", new
        {
            phone = phoneKind == "existing" ? realPhone : "+972509999999",
            password
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var text = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid phone or password", text);
        Assert.DoesNotContain("hash", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("secret", text, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Login_Twice_ProducesDifferentJtiClaims()
    {
        var user = await Fixture.SeedUserAsync("DONOR", "+972507777777");
        var first = await Fixture.LoginAsync(user.Phone);
        var second = await Fixture.LoginAsync(user.Phone);
        var handler = new JwtSecurityTokenHandler();

        Assert.NotEqual(
            handler.ReadJwtToken(first).Id,
            handler.ReadJwtToken(second).Id);
    }

    [Fact]
    public async Task DonorProbe_ValidDonorToken_ReturnsIdentityClaims()
    {
        var user = await Fixture.SeedUserAsync("DONOR", "+972508888888");
        var token = await Fixture.LoginAsync(user.Phone);
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/test/donor").WithBearer(token);

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.ReadJsonAsync();
        Assert.Equal(user.Id.ToString(), body.GetProperty("userId").GetString());
        Assert.Equal("DONOR", body.GetProperty("role").GetString());
    }

    [Theory]
    [InlineData("/api/test/donor")]
    [InlineData("/api/test/admin")]
    [InlineData("/api/test/donor-or-admin")]
    [InlineData("/api/donations/show-donations")]
    public async Task ProtectedEndpoint_WithoutToken_ReturnsUnauthorized(string path)
    {
        using var response = await Client.GetAsync(path);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("expired")]
    [InlineData("signature")]
    [InlineData("issuer")]
    [InlineData("audience")]
    public async Task ProtectedEndpoint_InvalidToken_ReturnsUnauthorized(string defect)
    {
        var user = await Fixture.SeedUserAsync("DONOR", "+972509000001");
        var token = JwtTestTokens.Create(
            user.Id,
            "DONOR",
            expires: defect == "expired" ? DateTime.UtcNow.AddMinutes(-1) : null,
            key: defect == "signature" ? "different-test-signing-key-at-least-32-bytes-long" : null,
            issuer: defect == "issuer" ? "wrong-issuer" : null,
            audience: defect == "audience" ? "wrong-audience" : null);
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/test/donor").WithBearer(token);

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DonorEndpoint_RecipientRole_ReturnsForbidden()
    {
        var population = await Fixture.SeedPopulationTypeAsync();
        var user = await Fixture.SeedUserAsync(
            "RECIPIENT", "+972509000002", populationTypeId: population.Id);
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/test/donor")
            .WithBearer(await Fixture.LoginAsync(user.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DonorDonationEndpoint_TokenWithoutSubjectOrInvalidSubject_ReturnsUnauthorized()
    {
        foreach (var token in new[]
                 {
                     JwtTestTokens.Create(null, "DONOR"),
                     JwtTestTokens.CreateWithRawSubject("not-an-integer", "DONOR")
                 })
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/donations/show-donations")
                .WithBearer(token);
            using var response = await Client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }

    [Theory]
    [InlineData("DONOR", "/api/test/donor-or-admin", HttpStatusCode.OK)]
    [InlineData("ADMIN", "/api/test/donor-or-admin", HttpStatusCode.OK)]
    [InlineData("RECIPIENT", "/api/test/donor-or-admin", HttpStatusCode.Forbidden)]
    [InlineData("ADMIN", "/api/test/admin", HttpStatusCode.OK)]
    [InlineData("DONOR", "/api/test/admin", HttpStatusCode.Forbidden)]
    public async Task RoleProbe_EnforcesDeclaredRoles(
        string role, string path, HttpStatusCode expected)
    {
        var token = JwtTestTokens.Create(123, role);
        using var request = new HttpRequestMessage(HttpMethod.Get, path).WithBearer(token);
        using var response = await Client.SendAsync(request);
        Assert.Equal(expected, response.StatusCode);
    }
}
