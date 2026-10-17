# Quickstart: Jass MCP API Validation

This guide describes end-to-end validation for the API and its independent persistence/domain implementation. The API host is `src/JassApi/JassApi.csproj` and targets .NET 9. The initial API unit tests use xUnit, Moq and FluentAssertions. Entra ID permission values and safe database test access are environment prerequisites for authenticated data queries.

## Prerequisites

- A development connection to the existing database with read-only access.
- An Entra ID tenant and API app registration with the expected audience and permission configured for the test MCP client.
- A compatible MCP client capable of connecting over Streamable HTTP and providing an Entra ID access token.
- .NET 9 SDK.
- Entra ID tenant ID, API client ID, API audience, and at least one permission configured in `AzureAd:RequiredScopes` or `AzureAd:RequiredRoles`; without a required permission, protected endpoints intentionally reject all requests.
- SQL Server connection string with read-only access to a safe test database.

## Configure Local Development

From `src/JassApi`, use .NET user secrets or environment variables for `AzureAd:TenantId`, `AzureAd:ClientId`, `AzureAd:Audience`, `AzureAd:RequiredScopes` or `AzureAd:RequiredRoles`, and `AppSettings:ConnectionString`. Never put actual identity or database secrets in `appsettings.json` or commit them.

## Build, Test and Run

From the repository root:

```powershell
dotnet build JassApi.sln --verbosity minimal
dotnet test JassApi.sln --verbosity minimal
dotnet run --project src\JassApi\JassApi.csproj --urls https://localhost:7001
```

The anonymous health endpoint is `GET https://localhost:7001/health`. The protected MCP Streamable HTTP endpoint is `https://localhost:7001/mcp`. Development OpenAPI is available at `/openapi/v1.json`.

## Validate Domain and Data Behavior

1. Run the API unit and persistence-model tests with the .NET 9 SDK.
2. After the safe read-only SQL Server test database is configured, run the API query parity cases with representative player and round filters.
3. Confirm player IDs/names and filtered completed-round IDs match the stored tables; confirm response details preserve stored teams, player positions, trump types and nullable result values.
4. Temporarily make the database unavailable in a safe test environment and verify the request is reported as an operational failure, not a factual no-result answer.

**Expected**: No Jass rules are duplicated; service and existing application results agree for the shared test set; no database writes occur.

## Validate MCP Contract

1. Start the service with `dotnet run --project src\JassApi\JassApi.csproj --urls https://localhost:7001`.
2. Configure the MCP client with `https://localhost:7001/mcp` and a valid Entra ID access token for the configured API audience and scope/role. On a `401`, the server supplies the protected-resource metadata URL in `WWW-Authenticate` and publishes metadata at `/.well-known/oauth-protected-resource`.
3. Discover `list_players` and `list_player_rounds`; call them in sequence using an existing player ID. Verify round results include teams, players, stored results and trump rounds. Repeat round listing with opponent, date and training-camp filters, then try an unknown player ID and an invalid date range.
4. Confirm returned values and validation outcomes follow [contracts/mcp-tools.md](contracts/mcp-tools.md).

**Expected**: Tool discovery and calls succeed for valid authorized requests; filtered results contain only matching completed rounds with stored teams/results/trump rounds; no raw database query capability is exposed. The MCP client, not the API, maps natural-language questions to tool calls.

## Validate Entra ID Authorization

1. Call the protected MCP endpoint using a valid token with the required permission.
2. Repeat without a token, with an expired/invalid token, and with a valid token lacking the required permission.
3. Repeat equivalent checks for any regular API endpoint that returns protected Jass data.

**Expected**: Only valid, authorized callers receive protected data. Rejected calls disclose no Jass records and no token material.

## Validate Acceptance Targets

- Run the agreed representative question set and calculate the correct-result rate; target at least 95% (spec SC-001).
- Measure successful answer latency and confirm at least 95% complete within 3 seconds, excluding confirmed source delays (spec SC-002).
- Verify all unauthorized test cases disclose zero protected data (spec SC-003).
- Have test users find a player and request filtered round history without instructions; record completion rate (spec SC-004).
- Have test users distinguish unsupported/source-failure outcomes from reliable answers and record correct recognition rate (spec SC-005).
