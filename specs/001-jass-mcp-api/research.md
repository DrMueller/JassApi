# Research: Jass MCP API

## Repository and Domain Discovery

- **Decision**: The API has no project reference to `JassApp`. It owns a small EF Core model for the existing player, game-round, team, team-player and trump-round tables, plus the read query and calculations required by the MCP tools.
- **Rationale**: The sibling project combines required domain/data types with a Blazor Web App, OpenID Connect web UI, UI dependencies, and write repositories. Referencing its web project brings unnecessary coupling and transitive content. The API-owned entities map to the existing table names (`SpielerTable`, `CoiffeurSpielrundeTable`, `JassTeamTable`, `JassTeamSpielerTable`, `TrumpfrundeTable`) and preserve the stored column shapes without migrations or table changes.
- **Alternatives considered**: Project-reference the sibling Web SDK project or immediately extract its domain into a shared class library. Rejected because the request is for this API to be independent and the extraction would modify the sibling application. Copying only the small, required domain behavior into API-owned code is the chosen boundary.

## ASP.NET Core and MCP Hosting

- **Decision**: Host the MCP server inside the ASP.NET Core API service, using the official MCP C# SDK with Streamable HTTP transport; begin in stateless session mode unless server-initiated requests or session state are demonstrated as requirements. Keep the API host separate from the sibling Blazor Web App.
- **Rationale**: The official SDK documents ASP.NET Core hosting through `ModelContextProtocol.AspNetCore`, `AddMcpServer()`, `WithHttpTransport(...)`, and `MapMcp()`. Hosting both interfaces together minimizes deployment and duplicated service configuration. The SDK documents stateless mode as the default and recommended option when server-to-client requests are unnecessary.
- **Alternatives considered**: Project-reference the `JassApp` Web SDK project, add MCP to its Blazor host, or extract a shared class library there. Rejected because the API must be independent and no sibling project changes/references are desired.
- **Sources**: Context7 `/modelcontextprotocol/csharp-sdk`, transport and protected server documentation; https://github.com/modelcontextprotocol/csharp-sdk/blob/main/docs/concepts/transports/transports.md

## Entra ID Authentication and Authorization

- **Decision**: Validate Entra ID access tokens in the new API host and apply authorization to the MCP endpoint and every protected API endpoint. The existing Blazor app configures `AddMicrosoftIdentityWebApp` with OpenID Connect; use the API-oriented bearer integration, retaining Microsoft Identity Web if package compatibility permits. Prefer an API-exposed delegated scope for interactive/user clients; permit app permissions only if the deployment's client type requires them and the app registration explicitly grants them.
- **Rationale**: ASP.NET Core authorization middleware supports endpoint authorization and a fallback policy; the official MCP SDK supports ASP.NET Core authentication/authorization integration and `MapMcp().RequireAuthorization()`. Microsoft Identity Web provides Entra ID token validation and authorization helpers for ASP.NET Core APIs. Authentication alone does not establish permission to access Jass data, so endpoint authorization must be explicit.
- **Alternatives considered**: Anonymous MCP access or trusting the MCP client to enforce permissions. Rejected because the service itself must prevent unauthorized access. User-only delegated scopes are a sensible default, but the actual client/deployment model must be confirmed before tenant configuration is finalized.
- **Sources**: Context7 `/dotnet/aspnetcore.docs` (JWT bearer, fallback authorization policy); `/azuread/microsoft-identity-web` (Web API quickstart and scopes/app permissions); `/modelcontextprotocol/csharp-sdk` (protected MCP server sample).

## Domain and Data Access

- **Decision**: Expose a narrow Jass question capability through MCP that calls existing application/domain behavior and reads from the existing database through its established data-access boundary. Do not expose arbitrary database queries and do not duplicate business rules.
- **Rationale**: This preserves the authoritative source and produces consistent results with the current Blazor application while limiting data exposure and avoiding an LLM-facing raw database interface.
- **Alternatives considered**: Return raw database access to the model or reimplement rule handling in MCP tools. Rejected due to access-control, correctness, and maintenance risks.

## Composition Boundary for Existing Application

- **Decision**: Use only API-owned application and persistence types. Keep exact SQL table names and existing stored fields; queries project read models and never expose EF entities through MCP.
- **Rationale**: This ensures no compile-time/runtime dependency on the Blazor project and avoids copying its authentication/UI composition, SQL diagnostic logging, sensitive-data logging, or write repositories.
- **Alternatives considered**: Invoke all sibling DI setup or make a shared class library in the sibling repository. Both would couple this API change back to `JassApp`; excluded by the user's explicit independence requirement.

## Interface Shape

- **Decision**: Expose two structured tools: `list_players` and `list_player_rounds` with optional opponent/date/training-camp filters and full team/player/trump-round results. Natural-language interpretation stays with the MCP client.
- **Rationale**: The tools match the existing SQL model and provide a discoverable flow from player lookup to filtered history. Round results include the stored teams and trump rounds, so a separate details tool would duplicate data.
- **Alternatives considered**: A natural-language `ask_jass` tool that attempts server-side question interpretation, one tool per database table, or a generic SQL/search tool. Rejected because no server-side natural-language engine exists and database-oriented tools expose implementation details and excessive access.

## Plan Decisions and Remaining Implementation Inputs

The existing database schema and needed domain behavior were inspected in `JassApp`; the API owns its implementation and has no build/runtime project dependency on it. Remaining deployment inputs are safe test database access and the actual Entra ID tenant, API audience and delegated scope or app role. Until a permission is configured, the authorization policy intentionally rejects all callers. Do not inspect or copy connection-string or identity-secret values into this specification.
