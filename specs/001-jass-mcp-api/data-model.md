# Data Model: Stored Jass Data MCP

The service is read-only and introduces no persistent business data. API-owned EF Core entities map the existing SQL Server tables to bounded MCP results. Existing table and column names remain authoritative; no migration or schema update is performed.

## Existing Source Entities

### Spieler

- **Source**: API-owned `SpielerEntity`, mapped to existing `SpielerTable`.
- **Fields exposed**: `id` (stable integer identifier) and `name`.
- **Rules**: IDs and names are sourced from stored players. Listing all players returns an empty array when there are no records.

### Coiffeur-Spielrunde

- **Source**: API-owned `CoiffeurSpielrundeEntity`, mapped to existing `CoiffeurSpielrundeTable`.
- **Fields exposed**: round ID, `GestartetAm`, `Punktewert`, round options including `IsJassTrainingslager`, teams and trump rounds.
- **Selection rules**: Query by required player ID and optional opponent ID, inclusive start-time boundaries, and optional training-camp flag. Return only finished rounds with valid team data (exactly two teams and two players per team). Preserve the existing specification's invalid-team exclusion behavior.

### Jass-Team and Team-Spieler

- **Source**: API-owned `JassTeamEntity` and `JassTeamSpielerEntity`, mapped to existing `JassTeamTable` and `JassTeamSpielerTable`.
- **Fields exposed**: team type and exactly two players, including player ID, name, position and starting-player flag.
- **Relationships**: Each returned round has two teams; each team has two players.

### Coiffeur-Trumpfrunde

- **Source**: API-owned `TrumpfrundeEntity`, mapped to existing `TrumpfrundeTable`.
- **Fields exposed**: trump-round ID, point modifier, trump type and stored result values/flags for both teams.
- **Validation**: Nullable team result fields remain nullable. Do not compute substitute values or claim incomplete rounds are finished.

## Request and Result Entities

### Player List Query

- **Fields**: None.
- **Validation**: No user-supplied identifiers or filters.
- **Result**: Read-only projection of stored player IDs and names from `SpielerTable`.

### Player Round Query

- **Fields**: required `playerId`; optional `opponentId`, inclusive `startedAfterOrAt`, inclusive `startedBeforeOrAt`, and `isTrainingCamp`.
- **Validation**: Both supplied player IDs must exist; player and opponent must differ. When both date bounds are present, the lower bound must not follow the upper bound. Reject invalid values before querying SQL Server.

### Player Round Result

- **Fields**: round identifier, start time, point value, training-camp flag, teams, trump rounds and request correlation identifier.
- **Relationships**: One result belongs to the requested player and includes two teams with two players each and its stored trump rounds.
- **Empty result**: A valid query with no matching completed rounds returns an empty collection; it is not an operational failure.
- **Operational errors**: Database unavailability or unexpected failures are surfaced as errors and never mapped to an empty collection.

### Entra ID Principal

- **Purpose**: Verified caller for protected API/MCP requests.
- **Validation**: Token issuer, audience, signature, lifetime and configured delegated scope/application permission are validated by the service. Identity is never accepted from tool parameters.

## Read-Only Data Flow

1. Authenticate and authorize the caller.
2. Validate structured tool arguments.
3. Query API-owned EF entities mapped to the existing player and round tables.
4. Map existing domain values to the stable MCP response contract without changing stored data or duplicating domain rules.
5. Return an empty list for valid no-match queries; report data-source failure as an operational error.
