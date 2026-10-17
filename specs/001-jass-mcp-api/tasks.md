# Tasks: Jass MCP API

**Input**: Design documents from `specs/001-jass-mcp-api/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/mcp-tools.md`, `quickstart.md`, `implementation-inputs.md`

**Tests**: Included to verify the independent acceptance scenarios, parity with existing queries, authentication, and read-only behavior. Write focused tests before the implementation tasks they cover.

**Organization**: Tasks are grouped by user story. US1 provides stored Jass-data tools; US2 secures access; US3 ensures existing data/domain behavior is reused. The end-to-end US1 flow depends on US2 and US3.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel without editing the same files or depending on unfinished tasks.
- **[Story]**: Required on user-story tasks only; setup, foundation, and polish tasks have no story label.
- Every task includes an exact file path.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Establish the API host and record the confirmed sibling-project integration inputs.

- [x] T001 Record in `specs/001-jass-mcp-api/implementation-inputs.md` that the API has no JassApp dependency, targets `net9.0`, maps its own EF Core entities to existing SQL tables, and requires deployment Entra ID permission plus safe test database access.
- [x] T002 Create `src/JassApi/JassApi.csproj` targeting `net9.0` with API-owned domain and persistence implementation; do not add a project reference to `JassApp`.
- [x] T003 [P] Create `src/JassApi/Program.cs` with the API host and register only API-owned query, persistence, authentication and MCP services.
- [x] T004 [P] Create `src/JassApi/appsettings.json` with non-secret local configuration keys and safe defaults; do not store client secrets, access tokens, database credentials, or production tenant values.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish shared response mapping, parameter validation, error behavior, and tests before adding MCP tools.

- [x] T005 Create `src/JassApi/Application/PlayerResponse.cs` with the stable `id` and `name` values returned from API-owned player records.
- [x] T006 Create `src/JassApi/Application/PlayerRoundQuery.cs` with required integer `playerId` and optional integer `opponentId`, inclusive `startedAfterOrAt`, inclusive `startedBeforeOrAt`, and nullable boolean `isTrainingCamp`; reject unknown IDs, identical player/opponent IDs, and lower date bounds after upper bounds before querying.
- [x] T007 Create `src/JassApi/Application/PlayerRoundResponse.cs` with round ID, start time, point value, training-camp flag, two teams (each containing team type and two players with ID/name/position/starting-player flag), and trump rounds with stored nullable results and flags for both teams.
- [x] T008 Create `src/JassApi/Infrastructure/ApiExceptionHandler.cs` to map invalid parameters and unknown IDs to protocol-valid client errors and SQL Server/internal failures to operational errors, never to an empty successful response.
- [x] T009 [P] Create `tests/JassApi.UnitTests/JassApi.UnitTests.csproj` targeting `net9.0`, using xUnit, Moq, FluentAssertions and a project reference to `src/JassApi/JassApi.csproj`.
- [ ] T010 [P] When safe test infrastructure is provisioned, create `tests/JassApi.IntegrationTests/JassApi.IntegrationTests.csproj` and `tests/JassApi.ContractTests/JassApi.ContractTests.csproj` targeting `net9.0`, following the xUnit conventions in `tests/JassApi.UnitTests/JassApi.UnitTests.csproj`.

**Checkpoint**: The host builds independently; API-owned entities map to the existing SQL schema and tests follow local xUnit conventions.

---

## Phase 3: User Story 3 - Reuse Existing Data and Domain Rules (Priority: P1)

**Goal**: Use API-owned SQL Server mappings and the required Jass result calculations, independent of JassApp.

**Independent Test**: For known safe data, API results agree with stored player/round behavior; returned rounds are completed with valid team structures; no Blazor project is loaded and no data is written.

### Tests for User Story 3

- [ ] T011 [P] [US3] Add player and round parity cases in `tests/JassApi.UnitTests/ExistingQueryParityUnitTests.cs` against the documented existing SQL behavior; cover player IDs/names, optional opponent/date/training-camp filters, completed rounds, team/player projections and stored trump-round values against a safe test database.
- [ ] T012 [P] [US3] Add composition-boundary cases in `tests/JassApi.UnitTests/ApiCompositionUnitTests.cs` proving API services do not require Blazor UI, Razor component, browser, speech-synthesis, or OpenID Connect web-app services.
- [ ] T013 [P] [US3] Add read-only verification in `tests/JassApi.UnitTests/ReadOnlyDataAccessUnitTests.cs` proving tool query flows do not call `SaveChangesAsync` or write repositories against a safe test database.

### Implementation for User Story 3

- [x] T014 [US3] Implement `src/JassApi/Application/JassDataQueryService.cs` using API-owned EF entities; list players and query rounds with required player ID, optional opponent/date/training-camp values, completed-round filtering and valid two-team/two-player structure.
- [x] T015 [US3] Implement `src/JassApi/Application/JassDataMapper.cs` to map API-owned player/round/team/trump-round entities to response records while preserving stored result values.
- [x] T016 [US3] Register API-owned `JassApiDbContextFactory`, EF model and read-only query service in `src/JassApi/Program.cs`; no JassApp factory, settings, or repository is referenced.
- [ ] T017 [US3] Once safe read-only SQL Server access is provisioned, run `tests/JassApi.UnitTests/ExistingQueryParityUnitTests.cs`, `tests/JassApi.UnitTests/ApiCompositionUnitTests.cs`, and `tests/JassApi.UnitTests/ReadOnlyDataAccessUnitTests.cs`; verify parity, UI independence, and zero writes.

**Checkpoint**: Player and round queries use API-owned types and read-only persistence while preserving the existing schema; no JassApp project reference remains.

---

## Phase 4: User Story 2 - Authenticate and Authorize Callers (Priority: P1)

**Goal**: Require a valid Entra ID token and configured API permission before player or round data is exposed.

**Independent Test**: A valid token with permission succeeds; no token, invalid token, and a valid token without permission are rejected with no protected data.

### Tests for User Story 2

- [ ] T018 [P] [US2] Add token and permission cases in `tests/JassApi.UnitTests/EntraAuthorizationUnitTests.cs` for authorized, missing-token, invalid-token, and underprivileged requests; assert no player/round content is returned on rejection.

### Implementation for User Story 2

- [x] T019 [US2] Configure Entra ID JWT bearer validation in `src/JassApi/Authentication/EntraIdAuthenticationExtensions.cs` using Microsoft Identity Web compatible with `net9.0`, with issuer, audience and tenant supplied by environment configuration and no secrets committed.
- [x] T020 [US2] Define the required delegated scope or application permission in `src/JassApi/Authentication/JassApiAuthorization.cs` and document the actual client model and permission name in `specs/001-jass-mcp-api/implementation-inputs.md` once the app registration owner supplies them.
- [x] T021 [US2] Add authentication/authorization middleware and an authenticated-user fallback policy to `src/JassApi/Program.cs`; allow anonymous access only to explicitly designated health/readiness endpoints.
- [ ] T022 [US2] Once a non-production Entra ID app registration and test identities are provisioned, run `tests/JassApi.UnitTests/EntraAuthorizationUnitTests.cs`; verify rejected calls disclose no protected content and advertise protected-resource metadata.

**Checkpoint**: Every protected API/MCP endpoint enforces token validation and the configured permission before querying or returning stored data.

---

## Phase 5: User Story 1 - Query Stored Jass Data Through MCP (Priority: P1) - MVP

**Goal**: Authorized MCP clients can list players and retrieve completed player rounds with filters and stored round details.

**Dependencies**: Requires US2 authorization and US3's existing query/data mapping.

**Independent Test**: Discover and call both tools with an authorized client; verify list results, required player ID, optional opponent/date/training-camp filters, completed valid rounds including teams/results/trump rounds, valid empty arrays, validation errors and operational failure handling.

### Tests for User Story 1

- [ ] T023 [P] [US1] Add MCP contract cases in `tests/JassApi.UnitTests/PlayerDataToolsContractUnitTests.cs` for `list_players` and `list_player_rounds`, asserting input/result fields, team/trump-round details and empty-result semantics from `specs/001-jass-mcp-api/contracts/mcp-tools.md`.
- [ ] T024 [P] [US1] Once a non-production Entra ID app registration exists, add MCP endpoint authorization cases in `tests/JassApi.UnitTests/ProtectedMcpEndpointUnitTests.cs` proving both tools are discoverable/callable with permission and rejected without permission.
- [x] T025 [P] [US1] Add tool argument validation cases in `tests/JassApi.UnitTests/Application/PlayerRoundQueryValidatorUnitTests.cs` for invalid player IDs, self-opponent and reversed date bounds; verify invalid filters fail before database access.

### Implementation for User Story 1

- [x] T026 [US1] Add the official MCP ASP.NET Core SDK dependency to `src/JassApi/JassApi.csproj`, selecting a package version compatible with `net9.0` and recording the resolved version in `specs/001-jass-mcp-api/implementation-inputs.md`.
- [x] T027 [US1] Implement `list_players` in `src/JassApi/Mcp/PlayerDataTools.cs` to delegate to `JassDataQueryService` and return only existing stable player IDs and names.
- [x] T028 [US1] Implement `list_player_rounds` in `src/JassApi/Mcp/PlayerDataTools.cs` with required `playerId`, optional `opponentId`, inclusive `startedAfterOrAt`/`startedBeforeOrAt`, and optional `isTrainingCamp`; validate IDs/date range and return only matching completed valid rounds, including all teams, players, stored round results and trump rounds.
- [x] T029 [US1] Register MCP Streamable HTTP transport in `src/JassApi/Program.cs`, use stateless sessions unless server-initiated requests are required, register `list_players` and `list_player_rounds`, and protect the mapped MCP endpoint with the US2 authorization policy.
- [x] T030 [US1] Run the available `tests/JassApi.UnitTests/JassApi.UnitTests.csproj`; verify date/self-opponent validation, response mapping, nullable stored results, delegated-scope/app-role checks and fail-closed authorization configuration. Database parity and MCP protocol integration cases remain pending safe test database and Entra ID app registration setup.

**Checkpoint**: Both structured read-only tools work through MCP; the API does not perform natural-language interpretation, expose free SQL, or write to the database.

**Environment gate**: Database query parity/no-write scenarios and successful Entra ID-protected tool calls remain unverified until safe SQL Server test data and a non-production Entra ID app registration are provisioned. Complete T017-T024 and T033-T036 once those prerequisites are available.

---

## Phase 6: Polish & Cross-Cutting Validation

**Purpose**: Document use and verify security, performance, parity, usability and read-only behavior together.

- [x] T031 [P] Update `specs/001-jass-mcp-api/quickstart.md` with actual build/test/run commands, MCP endpoint URL, safe test data requirements and Entra ID setup using non-secret values.
- [x] T032 [P] Document tool names, parameters, API audience, required permission and secret-free configuration keys in `src/JassApi/README.md`.
- [ ] T033 Run the representative stored-data question/query set and record expected-result rate and p95 latency in `specs/001-jass-mcp-api/validation-results.md`; verify SC-001 (at least 95% correct) and SC-002 (at least 95% within 3 seconds, excluding confirmed source delays).
- [ ] T034 Run authorized/unauthorized token cases and record that no protected data is disclosed in `specs/001-jass-mcp-api/validation-results.md`; verify SC-003.
- [ ] T035 Run quickstart usability scenarios and record task completion/outcome recognition in `specs/001-jass-mcp-api/validation-results.md`; verify SC-004 and SC-005.
- [ ] T036 Run read-only integration checks and record that no database writes occurred in `specs/001-jass-mcp-api/validation-results.md`; verify SC-006.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: Begins immediately; T001 records API-owned schema mapping and outstanding environment prerequisites.
- **Foundational (Phase 2)**: Depends on the API host and blocks all user stories.
- **US3 (Phase 3)** and **US2 (Phase 4)**: Both depend on the foundation and can proceed in parallel, coordinating edits to `src/JassApi/Program.cs`.
- **US1 (Phase 5)**: Depends on US2 token/permission enforcement and US3 query/mapping services.
- **Polish (Phase 6)**: Depends on all three user stories.

### User Story Dependencies

- **US1 (P1)**: Depends on US2 authorization and US3 existing data/domain integration.
- **US2 (P1)**: Depends on the foundational host and configuration boundary; independently testable with Entra ID test identities.
- **US3 (P1)**: Depends on the foundational host and safe SQL Server test environment; independently testable against the API-owned mapping and read-only database access.

### Parallel Opportunities

- After T001-T002, T003 and T004 can run in parallel on separate files.
- After shared request/output boundaries, T009 and T010 can run in parallel on separate test projects.
- US3 parity/composition tests and US2 authorization tests can be prepared in parallel. Their implementations are independent except for coordinated `Program.cs` registrations.
- Within US1, T023-T025 test files can be authored in parallel. Both tool operations in `PlayerDataTools.cs` share a file and should be implemented together or sequenced; MCP registration follows them.
- T032 and T033 can run in parallel; acceptance measurements require an integrated host and environment.

## Parallel Example: Independent Foundation Stories

```text
After foundational tasks:
  Developer A: US3 parity/composition/read-only tests and query adapter (T011-T017)
  Developer B: US2 Entra ID tests and authorization policy (T018-T022)

After US2 and US3 checkpoints:
  Developer A: MCP contract and argument-validation tests (T023, T025)
  Developer B: protected endpoint tests and MCP tool/endpoint implementation (T024, T026-T030)
```

## Implementation Strategy

### MVP Scope

The MVP consists of the secured structured data tools **plus** their prerequisites: US2 Entra ID authorization and US3 API-owned read-only persistence/domain logic. The tools are `list_players` and `list_player_rounds`, where round results include teams, players and stored trump-round details. General Jass rules, server-side natural-language interpretation, arbitrary SQL and write operations are out of scope.

### Incremental Delivery

1. Establish the .NET 9 API host and shared mappings.
2. Complete US2 and US3 independently and validate each checkpoint.
3. Add the two MCP data tools in US1 and validate the contract.
4. Complete security, latency, usability, parity and no-write validation before release. The end-to-end checks require the safe database and Entra ID inputs in `specs/001-jass-mcp-api/implementation-inputs.md`.

## Format Validation

Every actionable task has the `- [ ] Tnnn` checklist prefix, sequential ID, `[P]` only where safe parallel work is possible, story labels for user-story phases, and at least one exact file path. Setup, foundational, and polish tasks intentionally omit story labels.
