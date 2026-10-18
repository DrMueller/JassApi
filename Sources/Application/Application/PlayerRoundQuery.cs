namespace JassApi.Application;

public sealed record PlayerRoundQuery(
    int PlayerId,
    int? OpponentId,
    DateTimeOffset? StartedAfterOrAt,
    DateTimeOffset? StartedBeforeOrAt,
    bool? IsTrainingCamp);