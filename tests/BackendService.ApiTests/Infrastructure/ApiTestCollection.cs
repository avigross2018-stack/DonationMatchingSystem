namespace BackendService.ApiTests.Infrastructure;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class ApiTestCollection : ICollectionFixture<ApiTestFixture>
{
    public const string Name = "API integration tests";
}
