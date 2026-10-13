# Backend API tests

The test project exercises the real ASP.NET Core HTTP pipeline and a real MySQL
8.4 database. It does not use EF InMemory or SQLite. The suite covers public
registration and login, JWT validation and role authorization, donation CRUD,
error contracts, persistence, matching calculations, worker events, and
concurrent matching.

## Prerequisites

- .NET SDK 10
- A running Docker daemon capable of pulling `mysql:8.4`

Run the full suite from the repository root:

```bash
dotnet test DonationMatchingSystem.slnx
```

The fixture starts one MySQL container for the API collection, creates the
seven-table EF schema inside that disposable database, and truncates all data
between tests. API integration tests deliberately run sequentially because
they share the database and the in-process matching worker. Unit tests remain
parallelizable.

If Docker is unavailable, point the suite at an explicitly disposable MySQL
8.4 database instead:

```bash
TEST_MYSQL_CONNECTION_STRING='Server=...;Port=...;Database=...;User=...;Password=...' \
  dotnet test tests/BackendService.ApiTests/BackendService.ApiTests.csproj
```

The supplied database is destructive test infrastructure: all seven mapped
tables are truncated between tests. Never use a development or production
connection string. The fixture never reads application secrets and injects a
test-only JWT key, issuer, and audience.

Useful focused runs:

```bash
dotnet test --filter FullyQualifiedName~AuthRegistrationTests
dotnet test --filter FullyQualifiedName~DonationApiTests
dotnet test --filter FullyQualifiedName~MatchingIntegrationTests
dotnet test --filter FullyQualifiedName~Unit
```

Docker startup failures are surfaced with a diagnostic explaining that Docker
or `TEST_MYSQL_CONNECTION_STRING` is required; the suite does not silently
fall back to an incompatible database provider.
