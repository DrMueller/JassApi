# Jass API and MCP Server

This self-contained .NET 9 ASP.NET Core host exposes read-only Jass data through an authenticated MCP Streamable HTTP endpoint. It has no project or runtime dependency on the sibling application.

## MCP Tools

- `list_players`: returns stored player IDs and names.
- `list_player_rounds`: returns completed rounds for a required `playerId`; optionally accepts `opponentId`, inclusive `startedAfterOrAt`, inclusive `startedBeforeOrAt`, and `isTrainingCamp`. Each result includes both teams, their players, stored team scores and trump-round results.

Natural-language interpretation belongs to the MCP client. This server does not expose arbitrary SQL, table browsing, Jass-data mutation, or general Jass rules.

## Endpoints

- `GET /health`: anonymous health check.
- `POST /mcp`: Entra ID bearer-authenticated MCP Streamable HTTP endpoint.
- `GET /openapi/v1.json`: development-only API OpenAPI document.

## Entra ID Configuration

Configure `AzureAd:Instance`, `AzureAd:TenantId`, `AzureAd:ClientId`, and `AzureAd:Audience` for the API app registration. Configure at least one of `AzureAd:RequiredScopes` (scope names as they appear in delegated `scp` claims, for example `jass.read`) or `AzureAd:RequiredRoles` (app-role `roles` claim values). Both permission lists are empty by default and the authorization policy fails closed until at least one real API permission is configured. Request tokens for this API's audience; do not use an ID token as an API access token.

## Database Configuration

Set `AppSettings:ConnectionString` to the existing SQL Server database using user secrets, a protected environment variable, or deployment secret store. Give the deployed identity only database `SELECT` rights for the tables required by the two tools. Never commit connection strings or identity secrets.

The API owns its EF Core entity model and maps it to the existing `SpielerTable`, `CoiffeurSpielrundeTable`, `JassTeamTable`, `JassTeamSpielerTable`, and `TrumpfrundeTable` SQL tables. It adds no migrations and does not enable sensitive-data SQL logging.

## Azure App Registration

Register this service as an Entra ID API, configure its application ID URI/audience, and expose a delegated scope such as `jass.read`. Grant that scope to the MCP client registration and add its exact value to `AzureAd:RequiredScopes`; for daemon clients, configure an app role and add its exact value to `AzureAd:RequiredRoles`. Select the appropriate permission model with the tenant administrator. Until at least one permission is configured, the service rejects callers even if the token is otherwise valid.

## Run Locally

From the repository root:

```powershell
dotnet run --project src\JassApi\JassApi.csproj --urls https://localhost:7001
```

See [`specs/001-jass-mcp-api/quickstart.md`](../../specs/001-jass-mcp-api/quickstart.md) for setup and validation scenarios.
