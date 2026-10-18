namespace JassApi.Application;

public static class PlayerRoundQueryValidator
{
    public const string InvalidPlayerIdMessage = "The player ID must be positive.";
    public const string InvalidOpponentIdMessage = "The opponent ID must be positive.";
    public const string InvalidDateRangeMessage = "The start date must not be later than the end date.";
    public const string SamePlayerAndOpponentMessage = "The player and opponent must be different.";

    public static IReadOnlyCollection<string> Validate(PlayerRoundQuery query)
    {
        var errors = new List<string>();

        if (query.PlayerId <= 0) errors.Add(InvalidPlayerIdMessage);

        if (query.OpponentId is <= 0) errors.Add(InvalidOpponentIdMessage);

        if (query.OpponentId == query.PlayerId) errors.Add(SamePlayerAndOpponentMessage);

        if (query.StartedAfterOrAt > query.StartedBeforeOrAt) errors.Add(InvalidDateRangeMessage);

        return errors;
    }
}