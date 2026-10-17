# Implementation Inputs: Jass MCP API

## Confirmed Inputs

- The API project and solution have no reference to the sibling `JassApp` project.
- Target framework: `.NET 9` (`net9.0`)
- Data store: existing SQL Server database via EF Core SQL Server 9.0.7
- API-owned read-only player and completed-round queries, EF entity types, and required Jass result calculations.
- Persistence: API-owned EF Core model maps to `SpielerTable`, `CoiffeurSpielrundeTable`, `JassTeamTable`, `JassTeamSpielerTable`, and `TrumpfrundeTable`, preserving stored columns and types; no migrations are created or applied.
- Test conventions: xUnit, Moq, FluentAssertions
- Identity package: Microsoft Identity Web 3.12.0
- MCP package: `ModelContextProtocol.AspNetCore` 1.4.1
- OpenAPI package: `Microsoft.AspNetCore.OpenApi` 9.0.5
- Hosting boundary: self-contained ASP.NET Core API host; no sibling Blazor application is loaded or required.

## Deployment Configuration Required

Supply actual values using a protected secret store, environment variables, or .NET user secrets, not committed files:

- `AzureAd:TenantId`, `AzureAd:ClientId`, and `AzureAd:Audience` for the API app registration.
- At least one `AzureAd:RequiredScopes` entry or `AzureAd:RequiredRoles` entry. Both lists are empty by default, so protected access fails closed until a permission is configured.
- `AppSettings:ConnectionString` for the existing SQL Server database. Grant the service identity only the read permissions needed by the tools.
- A safe test database containing representative players and completed rounds before data parity/no-write integration scenarios are run.
- A compatible MCP client authorized to acquire an access token for the API audience and required permission.

Never store tenant credentials, access tokens, client secrets, or database connection strings in this document, `appsettings.json`, or source control.
