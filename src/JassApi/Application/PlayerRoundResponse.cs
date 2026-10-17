namespace JassApi.Application;

/// <summary>Represents a completed Coiffeur round and its stored team and trump-round data.</summary>
public sealed record PlayerRoundResponse(
    int RoundId,
    DateTimeOffset StartedAt,
    int PointValue,
    bool IsTrainingCamp,
    IReadOnlyList<JassTeamResponse> Teams,
    IReadOnlyList<TrumpRoundResponse> TrumpRounds);

/// <summary>Represents one team and its two stored players.</summary>
public sealed record JassTeamResponse(
    string TeamType,
    IReadOnlyList<JassTeamPlayerResponse> Players);

/// <summary>Represents a player's stored position in a team.</summary>
public sealed record JassTeamPlayerResponse(
    int Id,
    string Name,
    string Position,
    bool IsStartingPlayer);

/// <summary>Represents one stored trump round and its results for both teams.</summary>
public sealed record TrumpRoundResponse(
    int Id,
    string TrumpType,
    int PointModifier,
    int? ResultTeam1,
    int? ResultTeam2,
    bool IsMatchTeam1,
    bool IsMatchTeam2,
    bool IsCounterMatchTeam1,
    bool IsCounterMatchTeam2);