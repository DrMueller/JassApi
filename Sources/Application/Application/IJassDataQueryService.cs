namespace JassApi.Application;

public interface IJassDataQueryService
{
    Task<IReadOnlyList<PlayerRoundResponse>> ListPlayerRoundsAsync(
        PlayerRoundQuery query,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<PlayerResponse>> ListPlayersAsync(CancellationToken cancellationToken);
}