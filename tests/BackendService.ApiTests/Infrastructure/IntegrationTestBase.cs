namespace BackendService.ApiTests.Infrastructure;

[Collection(ApiTestCollection.Name)]
public abstract class IntegrationTestBase(ApiTestFixture fixture) : IAsyncLifetime
{
    protected ApiTestFixture Fixture { get; } = fixture;
    protected HttpClient Client => Fixture.Client;

    public Task InitializeAsync() => Fixture.ResetAsync();
    public Task DisposeAsync() => Task.CompletedTask;
}
