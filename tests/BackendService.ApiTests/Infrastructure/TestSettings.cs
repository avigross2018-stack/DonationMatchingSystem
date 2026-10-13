namespace BackendService.ApiTests.Infrastructure;

internal static class TestSettings
{
    internal const string JwtKey = "integration-tests-only-signing-key-at-least-32-bytes-long";
    internal const string JwtIssuer = "DonationMatchingSystem.ApiTests";
    internal const string JwtAudience = "DonationMatchingSystem.ApiTests.Client";
}
