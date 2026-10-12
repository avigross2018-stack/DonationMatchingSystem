using BackendService.Data;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BackendService.ApiTests.Infrastructure;

public sealed class TestWebApplicationFactory(string connectionString)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var parsed = new MySqlConnector.MySqlConnectionStringBuilder(connectionString);

        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MYSQL_HOST"] = parsed.Server,
                ["MYSQL_PORT"] = parsed.Port.ToString(),
                ["MYSQL_ROOT_USER"] = parsed.UserID,
                ["MYSQL_ROOT_PASSWORD"] = parsed.Password,
                ["MYSQL_DATABASE"] = parsed.Database,
                ["JWT:KEY"] = TestSettings.JwtKey,
                ["JWT:ISSUER"] = TestSettings.JwtIssuer,
                ["JWT:AUDIENCE"] = TestSettings.JwtAudience,
                ["JWT:EXPIRATION_MINUTES"] = "15",
                ["ADMIN_SEED:NAME"] = string.Empty,
                ["ADMIN_SEED:PHONE"] = string.Empty,
                ["ADMIN_SEED:PASSWORD"] = string.Empty
            });
        });
        builder.ConfigureServices(services =>
        {
            services.PostConfigure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = TestSettings.JwtIssuer,
                        ValidateAudience = true,
                        ValidAudience = TestSettings.JwtAudience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(TestSettings.JwtKey)),
                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = "sub",
                        ClockSkew = TimeSpan.Zero
                    };
                });
        });
    }

    public async Task EnsureSchemaAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    public HttpClient CreateApiClient() => CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false,
        BaseAddress = new Uri("https://localhost")
    });
}
