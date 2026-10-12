using BackendService.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
