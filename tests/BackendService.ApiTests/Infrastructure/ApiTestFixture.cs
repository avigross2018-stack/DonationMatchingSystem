using BackendService.Data;
using BackendService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MySql;

namespace BackendService.ApiTests.Infrastructure;

public sealed class ApiTestFixture : IAsyncLifetime
{
    private MySqlContainer? _container;

    public TestWebApplicationFactory Factory { get; private set; } = null!;
    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var explicitConnection = Environment.GetEnvironmentVariable(
            "TEST_MYSQL_CONNECTION_STRING");

        string connectionString;
        if (!string.IsNullOrWhiteSpace(explicitConnection))
        {
            connectionString = explicitConnection;
        }
        else
        {
            _container = new MySqlBuilder("mysql:8.4")
                .WithDatabase("donation_tests")
                .WithUsername("test_user")
                .WithPassword("test_password")
                .Build();

            try
            {
                await _container.StartAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Integration tests require Docker, or set TEST_MYSQL_CONNECTION_STRING " +
                    "to an isolated disposable MySQL 8.4 database.", ex);
            }

            connectionString = _container.GetConnectionString();
        }

        Factory = new TestWebApplicationFactory(connectionString);
        await Factory.EnsureSchemaAsync();
        Client = Factory.CreateApiClient();
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();
        if (Factory is not null)
            await Factory.DisposeAsync();
        if (_container is not null)
            await _container.DisposeAsync();
    }

    public async Task ResetAsync()
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.OpenConnectionAsync();
        try
        {
            await db.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0;");
            await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE `Redemptions`;");
            await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE `Donations`;");
            await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE `Recipients`;");
            await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE `Admins`;");
            await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE `Donors`;");
            await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE `PopulationTypes`;");
            await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE `Users`;");
            await db.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 1;");
        }
        finally
        {
            await db.Database.CloseConnectionAsync();
        }
    }

    public async Task<T> WithDbAsync<T>(Func<AppDbContext, Task<T>> action)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await action(db);
    }

    public async Task WithDbAsync(Func<AppDbContext, Task> action)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await action(db);
    }

    public Task<PopulationType> SeedPopulationTypeAsync(
        bool active = true, decimal baseAmount = 1_000m, string? name = null) =>
        WithDbAsync(async db =>
        {
            var entity = new PopulationType
            {
                Name = name ?? $"Population-{Guid.NewGuid():N}",
                Description = "API test population",
                BaseEligibilityAmount = baseAmount,
                IsActive = active
            };
            db.PopulationTypes.Add(entity);
            await db.SaveChangesAsync();
            return entity;
        });

    public Task<User> SeedUserAsync(
        string role, string phone, string password = "Passw0rd!", bool active = true,
        bool createProfile = true, int? populationTypeId = null,
        decimal eligibility = 1_000m) => WithDbAsync(async db =>
    {
        var user = new User
        {
            Name = $"Test {role}",
            Phone = phone,
            UserType = role,
            IsActive = active,
            CreatedAt = DateTime.UtcNow
        };
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, password);
        db.Users.Add(user);

        if (createProfile)
        {
            if (role == "DONOR") db.Donors.Add(new Donor { User = user });
            if (role == "ADMIN") db.Admins.Add(new Admin { User = user });
            if (role == "RECIPIENT")
            {
                if (populationTypeId is null)
                    throw new ArgumentException("Recipient requires a population type.");
                db.Recipients.Add(new Recipient
                {
                    User = user,
                    PopulationTypeId = populationTypeId.Value,
                    IncomeAmount = 0,
                    AmountOfChildren = 0,
                    EligibilityAmount = eligibility,
                    RemainingEligibility = eligibility
                });
            }
        }

        await db.SaveChangesAsync();
        return user;
    });

    public async Task<string> LoginAsync(string phone, string password = "Passw0rd!")
    {
        using var response = await Client.PostAsJsonAsync("/api/Auth/login", new
        {
            phone,
            password
        });
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("accessToken").GetString()!;
    }
}
