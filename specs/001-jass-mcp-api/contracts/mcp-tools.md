# MCP Tool Contract: Stored Jass Data

## Transport and Access

- **Protocol**: MCP over Streamable HTTP, hosted by the ASP.NET Core API service.
- **Authentication**: Entra ID access token for the API's configured audience.
- **Authorization**: Caller must hold the delegated scope or application permission configured for the deployed client model.
- **Data access**: Read-only queries through API-owned EF Core mappings to the existing SQL tables. No arbitrary SQL, table browsing, schema migration, or database mutation tool is exposed.
- **Natural-language interpretation**: Performed by the MCP client. Tools accept structured parameters and return data; they do not accept arbitrary natural-language questions for server-side interpretation.

## Tool: `list_players`

### Purpose

Return stored players so a client can resolve names to stable player IDs before requesting game history.

### Input

No arguments.

### Result

An array of players:

| Field | Type | Required | Meaning |
|-------|------|----------|---------|
| `id` | integer | Yes | Existing stable player identifier. |
| `name` | string | Yes | Existing player name. |

No matching players is represented by an empty array.

## Tool: `list_player_rounds`

### Purpose

Return completed stored Coiffeur rounds for one player, with optional opponent, date, and training-camp filters.

### Input

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| `playerId` | integer | Yes | Must identify an existing player. |
| `opponentId` | integer | No | If provided, must identify an existing player on the opposing team. |
| `startedAfterOrAt` | date-time | No | Inclusive lower bound for round start time. |
| `startedBeforeOrAt` | date-time | No | Inclusive upper bound for round start time; must not precede `startedAfterOrAt` when both are supplied. |
| `isTrainingCamp` | boolean | No | If provided, include only rounds with this stored training-camp value. |

The service rejects unknown player IDs, a player used as their own opponent, and an invalid date range before querying the data source.

### Result

An array of completed rounds:

| Field | Type | Required | Meaning |
|-------|------|----------|---------|
| `roundId` | integer | Yes | Existing stable round identifier. |
| `startedAt` | date-time | Yes | Stored round start time. |
| `pointValue` | integer | Yes | Stored round point value. |
| `isTrainingCamp` | boolean | Yes | Stored training-camp flag. |
| `teams` | array | Yes | The two teams, each with team type and two players (`id`, `name`, `position`, `isStartingPlayer`). |
| `trumpRounds` | array | Yes | Stored trump rounds with trump type, point modifier, and nullable result/matsch/counter-matsch values for both teams. |

Rounds are returned only if the existing domain rule considers them finished and their team data has exactly two teams with two players each. A valid query with no matching rounds returns an empty array.

## Errors

- **Unauthorized**: Reject the HTTP/MCP request before returning protected player or round data.
- **Invalid input**: Return a protocol-valid validation error identifying invalid parameters; do not query with invalid values.
- **Unknown player ID**: Return a not-found/validation error; do not silently return an empty result.
- **Data-source or internal failure**: Return a protocol-valid operational error; do not translate it to an empty result.

## Compatibility Notes

The MCP SDK supplies protocol framing and tool discovery/call semantics. The tools above define the application-level contracts. Property naming and wire encodings should follow the SDK's JSON schema conventions while preserving the meanings and validation rules specified here.
