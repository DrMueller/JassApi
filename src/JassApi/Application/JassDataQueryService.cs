using JassApi.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JassApi.Application;

public sealed class JassDataQueryService(IJassApiDbContextFactory contextFactory) : IJassDataQueryService
{
    public async Task<IReadOnlyList<PlayerRoundResponse>> ListPlayerRoundsAsync(
        PlayerRoundQuery query,
        CancellationToken cancellationToken)
    {
        var errors = PlayerRoundQueryValidator.Validate(query);
        if (errors.Count > 0) throw new ArgumentException(string.Join(" ", errors));

        cancellationToken.ThrowIfCancellationRequested();
        using var context = contextFactory.Create();
        var requiredPlayerIds = query.OpponentId.HasValue
            ? new[] { query.PlayerId, query.OpponentId.Value }
            : [query.PlayerId];
        var existingPlayerIds = await context.Players
            .Where(player => requiredPlayerIds.Contains(player.Id))
            .Select(player => player.Id)
            .ToListAsync(cancellationToken);

        if (existingPlayerIds.Count != requiredPlayerIds.Distinct().Count())
            throw new KeyNotFoundException("The requested player or opponent does not exist.");

        var rounds = await context.Rounds
            .AsNoTracking()
            .Include(round => round.JassTeams)
            .ThenInclude(team => team.JassTeamSpieler)
            .ThenInclude(teamPlayer => teamPlayer.Spieler)
            .Include(round => round.Trumpfrunden)
            .Where(round =>
                round.JassTeams.Any(team => team.JassTeamSpieler.Any(player => player.SpielerId == query.PlayerId)))
            .Where(round => !query.OpponentId.HasValue || round.JassTeams.Any(playerTeam =>
                playerTeam.JassTeamSpieler.Any(player => player.SpielerId == query.PlayerId)
                && round.JassTeams.Any(opponentTeam =>
                    opponentTeam.JassTeamTyp != playerTeam.JassTeamTyp
                    && opponentTeam.JassTeamSpieler.Any(player => player.SpielerId == query.OpponentId.Value))))
            .Where(round => !query.StartedAfterOrAt.HasValue ||
                            round.GestartetAm >= query.StartedAfterOrAt.Value.LocalDateTime)
            .Where(round => !query.StartedBeforeOrAt.HasValue ||
                            round.GestartetAm <= query.StartedBeforeOrAt.Value.LocalDateTime)
            .Where(round => !query.IsTrainingCamp.HasValue || round.IsJassTrainingslager == query.IsTrainingCamp.Value)
            .Where(round => round.Trumpfrunden.Count > 0
                            && round.JassTeams.Count == 2
                            && round.JassTeams.All(team => team.JassTeamSpieler.Count == 2)
                            && round.Trumpfrunden.All(trumpRound =>
                                trumpRound.ResultatTeam1.HasValue && trumpRound.ResultatTeam2.HasValue))
            .ToListAsync(cancellationToken);

        return rounds
            .OrderByDescending(round => round.GestartetAm)
            .Select(JassDataMapper.MapRound)
            .ToArray();
    }

    public async Task<IReadOnlyList<PlayerResponse>> ListPlayersAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var context = contextFactory.Create();
        var players = await context.Players
            .OrderBy(player => player.Name)
            .ToListAsync(cancellationToken);

        return players.Select(JassDataMapper.MapPlayer).ToArray();
    }
}