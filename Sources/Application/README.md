# Jass API and MCP Server

This self-contained .NET 9 ASP.NET Core host exposes read-only Jass data through an authenticated MCP Streamable HTTP endpoint. It has no project or runtime dependency on the sibling application.

## MCP Tools

- `list_players`: returns stored player IDs and names.
- `list_player_rounds`: returns completed rounds for a required `playerId`; optionally accepts `opponentId`, inclusive `startedAfterOrAt`, inclusive `startedBeforeOrAt`, and `isTrainingCamp`. Each result includes both teams, their players, stored team scores and trump-round results.

Natural-language interpretation belongs to the MCP client. This server does not expose arbitrary SQL, table browsing, Jass-data mutation, or general Jass rules.

## Endpoints

- `GET /health`: anonymous health check.
- `POST /mcp`: API-key-authenticated MCP Streamable HTTP endpoint.
- `GET /openapi/v1.json`: development-only API OpenAPI document.

## Authentifizierung

Send the configured key in the `X-Api-Key` request header. The service requires HTTPS and rejects requests without a valid key. It fails during startup if no key is configured.

For local development, keep the key out of source control and configure it with User Secrets:

```powershell
dotnet user-secrets set "AppSettings:ApiKey" "<key>" --project Sources\Application\JassApi.csproj
```

For an Azure Web App, configure the application setting `AppSettings__ApiKey`. To allow zero-downtime rotation, configure overlapping keys as `AppSettings__ApiKeys__0`, `AppSettings__ApiKeys__1`, and so on; both `AppSettings:ApiKey` and all entries in `AppSettings:ApiKeys` are accepted. Remove the old key after all clients have switched.

When TLS terminates at the Azure App Service proxy, set `ASPNETCORE_FORWARDEDHEADERS_ENABLED=true` so the application recognizes the original HTTPS scheme before redirect/authentication middleware. Only enable this when the trusted proxy controls the forwarded headers.

## Database Configuration

Set `AppSettings:ConnectionString` to the existing SQL Server database using user secrets, a protected environment variable, or deployment secret store. Give the deployed identity only database `SELECT` rights for the tables required by the two tools. Never commit connection strings or identity secrets.

The API owns its EF Core entity model and maps it to the existing `SpielerTable`, `CoiffeurSpielrundeTable`, `JassTeamTable`, `JassTeamSpielerTable`, and `TrumpfrundeTable` SQL tables. It adds no migrations and does not enable sensitive-data SQL logging.

## Run Locally

From the repository root:

```powershell
dotnet run --project src\JassApi\JassApi.csproj --urls https://localhost:7001
```

See [`specs/001-jass-mcp-api/quickstart.md`](../../specs/001-jass-mcp-api/quickstart.md) for setup and validation scenarios.
