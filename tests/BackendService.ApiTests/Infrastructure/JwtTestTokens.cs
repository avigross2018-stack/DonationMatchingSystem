using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BackendService.ApiTests.Infrastructure;

internal static class JwtTestTokens
{
    internal static string Create(
        int? userId,
        string role,
        DateTime? expires = null,
        string? key = null,
        string? issuer = null,
        string? audience = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        if (userId is not null)
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, userId.Value.ToString()));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key ?? TestSettings.JwtKey)),
            SecurityAlgorithms.HmacSha256);

        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            issuer ?? TestSettings.JwtIssuer,
            audience ?? TestSettings.JwtAudience,
            claims,
            expires: expires ?? DateTime.UtcNow.AddMinutes(10),
            signingCredentials: credentials));
    }

    internal static string CreateWithRawSubject(string subject, string role)
    {
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSettings.JwtKey)),
            SecurityAlgorithms.HmacSha256);
        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            TestSettings.JwtIssuer,
            TestSettings.JwtAudience,
            [new Claim(JwtRegisteredClaimNames.Sub, subject), new Claim(ClaimTypes.Role, role)],
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: credentials));
    }
}
