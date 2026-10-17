using JassApi.Application;

namespace JassApi.UnitTests.Application;

public partial class PlayerRoundQueryValidatorUnitTests
{
    private const int _playerId = 17;
    private static readonly DateTimeOffset _earlierDate = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset _laterDate = new(2026, 2, 1, 0, 0, 0, TimeSpan.Zero);

    private static PlayerRoundQuery CreateQuery(
        int playerId = _playerId,
        int? opponentId = null,
        DateTimeOffset? startedAfterOrAt = null,
        DateTimeOffset? startedBeforeOrAt = null)
    {
        return new PlayerRoundQuery(playerId, opponentId, startedAfterOrAt, startedBeforeOrAt, null);
    }
}