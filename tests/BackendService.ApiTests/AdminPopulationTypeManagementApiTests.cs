using BackendService.ApiTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BackendService.ApiTests;

public sealed class AdminPopulationTypeManagementApiTests(
    ApiTestFixture fixture) : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task GetAll_Admin_ReturnsActiveAndInactivePopulationTypes()
    {
        var active = await Fixture.SeedPopulationTypeAsync(
            active: true,
            baseAmount: 1_000m);

        var inactive = await Fixture.SeedPopulationTypeAsync(
            active: false,
            baseAmount: 2_000m);

        var admin = await Fixture.SeedUserAsync(
            "ADMIN",
            "+972533400001");

        using var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/api/admin/population-types")
            .WithBearer(await Fixture.LoginAsync(admin.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.ReadJsonAsync();
        var items = json.EnumerateArray().ToArray();

        Assert.Equal(2, items.Length);

        var activeItem = Assert.Single(
            items,
            item => item.GetProperty("id").GetInt32() == active.Id);

        Assert.True(activeItem.GetProperty("isActive").GetBoolean());
        Assert.Equal(
            1_000m,
            activeItem.GetProperty("baseEligibilityAmount").GetDecimal());

        var inactiveItem = Assert.Single(
            items,
            item => item.GetProperty("id").GetInt32() == inactive.Id);

        Assert.False(inactiveItem.GetProperty("isActive").GetBoolean());
        Assert.Equal(
            2_000m,
            inactiveItem.GetProperty("baseEligibilityAmount").GetDecimal());
    }

    [Fact]
    public async Task Create_ValidPayload_TrimsFieldsAndPersistsPopulationType()
    {
        var admin = await Fixture.SeedUserAsync(
            "ADMIN",
            "+972533400002");

        using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "/api/admin/population-types")
        {
            Content = JsonContent.Create(new
            {
                name = "  Community Support  ",
                description = "  Support for eligible families  ",
                baseEligibilityAmount = 2_500m
            })
        }
            .WithBearer(await Fixture.LoginAsync(admin.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var json = await response.ReadJsonAsync();
        var populationTypeId = json.GetProperty("id").GetInt32();

        Assert.True(populationTypeId > 0);
        Assert.Equal(
            "Community Support",
            json.GetProperty("name").GetString());
        Assert.Equal(
            "Support for eligible families",
            json.GetProperty("description").GetString());
        Assert.Equal(
            2_500m,
            json.GetProperty("baseEligibilityAmount").GetDecimal());
        Assert.True(json.GetProperty("isActive").GetBoolean());

        await Fixture.WithDbAsync(async db =>
        {
            var persisted = await db.PopulationTypes
                .AsNoTracking()
                .SingleAsync(x => x.Id == populationTypeId);

            Assert.Equal("Community Support", persisted.Name);
            Assert.Equal(
                "Support for eligible families",
                persisted.Description);
            Assert.Equal(2_500m, persisted.BaseEligibilityAmount);
            Assert.True(persisted.IsActive);
        });
    }

    [Fact]
    public async Task Create_DuplicateName_ReturnsConflictWithoutCreatingAnotherType()
    {
        var existing = await Fixture.SeedPopulationTypeAsync();
        var admin = await Fixture.SeedUserAsync(
            "ADMIN",
            "+972533400003");

        using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "/api/admin/population-types")
        {
            Content = JsonContent.Create(new
            {
                name = existing.Name,
                description = "Duplicate",
                baseEligibilityAmount = 1_500m
            })
        }
            .WithBearer(await Fixture.LoginAsync(admin.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var count = await Fixture.WithDbAsync(db =>
            db.PopulationTypes.CountAsync());

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task Create_NegativeBaseEligibilityAmount_ReturnsBadRequest()
    {
        var admin = await Fixture.SeedUserAsync(
            "ADMIN",
            "+972533400004");

        using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "/api/admin/population-types")
        {
            Content = JsonContent.Create(new
            {
                name = "Invalid Population",
                description = "Invalid base amount",
                baseEligibilityAmount = -1m
            })
        }
            .WithBearer(await Fixture.LoginAsync(admin.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var count = await Fixture.WithDbAsync(db =>
            db.PopulationTypes.CountAsync());

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task Update_ExistingRecipientEligibility_RemainsUnchanged()
    {
        var population = await Fixture.SeedPopulationTypeAsync(
            baseAmount: 1_000m);

        var recipientUser = await Fixture.SeedUserAsync(
            "RECIPIENT",
            "+972533400005",
            populationTypeId: population.Id,
            eligibility: 777m);

        var recipientId = await Fixture.WithDbAsync(db =>
            db.Recipients
                .Where(x => x.UserId == recipientUser.Id)
                .Select(x => x.Id)
                .SingleAsync());

        var admin = await Fixture.SeedUserAsync(
            "ADMIN",
            "+972533400006");

        using var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"/api/admin/population-types/{population.Id}")
        {
            Content = JsonContent.Create(new
            {
                name = population.Name,
                description = "Updated description",
                baseEligibilityAmount = 3_000m
            })
        }
            .WithBearer(await Fixture.LoginAsync(admin.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.ReadJsonAsync();

        Assert.Equal(
            3_000m,
            json.GetProperty("baseEligibilityAmount").GetDecimal());
        Assert.Equal(
            "Updated description",
            json.GetProperty("description").GetString());

        await Fixture.WithDbAsync(async db =>
        {
            var updatedPopulation = await db.PopulationTypes
                .AsNoTracking()
                .SingleAsync(x => x.Id == population.Id);

            Assert.Equal(3_000m, updatedPopulation.BaseEligibilityAmount);
            Assert.Equal("Updated description", updatedPopulation.Description);

            // Existing eligibility is not recalculated automatically.
            var recipient = await db.Recipients
                .AsNoTracking()
                .SingleAsync(x => x.Id == recipientId);

            Assert.Equal(777m, recipient.EligibilityAmount);
            Assert.Equal(777m, recipient.RemainingEligibility);
        });
    }

    [Fact]
    public async Task Update_DuplicateName_ReturnsConflictWithoutChangingTarget()
    {
        var first = await Fixture.SeedPopulationTypeAsync();
        var second = await Fixture.SeedPopulationTypeAsync(
            baseAmount: 2_000m);

        var admin = await Fixture.SeedUserAsync(
            "ADMIN",
            "+972533400007");

        using var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"/api/admin/population-types/{second.Id}")
        {
            Content = JsonContent.Create(new
            {
                name = first.Name,
                description = "Must not be saved",
                baseEligibilityAmount = 5_000m
            })
        }
            .WithBearer(await Fixture.LoginAsync(admin.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        await Fixture.WithDbAsync(async db =>
        {
            var unchanged = await db.PopulationTypes
                .AsNoTracking()
                .SingleAsync(x => x.Id == second.Id);

            Assert.Equal(second.Name, unchanged.Name);
            Assert.Equal(2_000m, unchanged.BaseEligibilityAmount);
            Assert.Equal(second.Description, unchanged.Description);
        });
    }

    [Fact]
    public async Task UpdateStatus_DisablePopulationType_RemainsVisibleToAdmin()
    {
        var population = await Fixture.SeedPopulationTypeAsync(
            active: true);

        var admin = await Fixture.SeedUserAsync(
            "ADMIN",
            "+972533400008");

        using var updateRequest = new HttpRequestMessage(
                HttpMethod.Patch,
                $"/api/admin/population-types/{population.Id}/status")
        {
            Content = JsonContent.Create(new
            {
                isActive = false
            })
        }
            .WithBearer(await Fixture.LoginAsync(admin.Phone));

        using var updateResponse = await Client.SendAsync(updateRequest);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updatedJson = await updateResponse.ReadJsonAsync();

        Assert.False(updatedJson.GetProperty("isActive").GetBoolean());

        // Verify that inactive types remain available in administrative views.
        using var getRequest = new HttpRequestMessage(
                HttpMethod.Get,
                "/api/admin/population-types")
            .WithBearer(await Fixture.LoginAsync(admin.Phone));

        using var getResponse = await Client.SendAsync(getRequest);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var items = (await getResponse.ReadJsonAsync())
            .EnumerateArray()
            .ToArray();

        var inactiveItem = Assert.Single(
            items,
            item => item.GetProperty("id").GetInt32() == population.Id);

        Assert.False(inactiveItem.GetProperty("isActive").GetBoolean());

        await Fixture.WithDbAsync(async db =>
        {
            var persisted = await db.PopulationTypes
                .AsNoTracking()
                .SingleAsync(x => x.Id == population.Id);

            Assert.False(persisted.IsActive);
        });
    }

    [Fact]
    public async Task GetAll_DonorToken_ReturnsForbidden()
    {
        var donor = await Fixture.SeedUserAsync(
            "DONOR",
            "+972533400009");

        using var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/api/admin/population-types")
            .WithBearer(await Fixture.LoginAsync(donor.Phone));

        using var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_Unauthenticated_ReturnsUnauthorized()
    {
        using var response = await Client.GetAsync(
            "/api/admin/population-types");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
