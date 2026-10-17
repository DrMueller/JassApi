using JassApi.Persistence.Entities;

namespace JassApi.Application;

public static class JassDomainCalculations
{
    public static int CalculateMatches(IReadOnlyList<TrumpRoundResponse> trumpRounds, JassTeamTyp teamType)
    {
        var matches = trumpRounds.Count(round => IsMatch(round, teamType));
        var opponentType = GetOpponent(teamType);
        var counterMatches = trumpRounds.Count(round => IsCounterMatch(round, opponentType));
        var heartsRound = trumpRounds.Single(round => round.TrumpType == "Herz");

        if (heartsRound.ResultTeam1.HasValue && heartsRound.ResultTeam2.HasValue)
        {
            var teamPoints = GetResult(heartsRound, teamType);
            var opponentPoints = GetResult(heartsRound, opponentType);
            if (teamPoints > opponentPoints) matches++;
        }

        return matches + counterMatches * 2;
    }

    public static int? CalculateTeamPoints(
        IReadOnlyList<TrumpRoundResponse> trumpRounds,
        JassTeamTyp teamType,
        bool isTrainingCamp)
    {
        if (trumpRounds.Count == 0 ||
            trumpRounds.Any(round => round.ResultTeam1 is null || round.ResultTeam2 is null)) return null;

        var teamPoints = 0;
        var opponentPoints = 0;
        foreach (var trumpRound in trumpRounds)
        {
            var ownResult = GetResult(trumpRound, teamType);
            var opposingResult = GetResult(trumpRound, GetOpponent(teamType));
            teamPoints += Math.Max(ownResult - opposingResult, 0) * trumpRound.PointModifier;
            opponentPoints += Math.Max(opposingResult - ownResult, 0) * trumpRound.PointModifier;
        }

        if (!isTrainingCamp)
        {
            var teamMatsches = CalculateMatches(trumpRounds, teamType);
            var opponentMatsches = CalculateMatches(trumpRounds, GetOpponent(teamType));
            teamPoints += (teamMatsches - opponentMatsches) * 20;
        }

        return teamPoints - opponentPoints;
    }

    private static JassTeamTyp GetOpponent(JassTeamTyp teamType)
    {
        return teamType == JassTeamTyp.Team1 ? JassTeamTyp.Team2 : JassTeamTyp.Team1;
    }

    private static int GetResult(TrumpRoundResponse round, JassTeamTyp teamType)
    {
        return (teamType == JassTeamTyp.Team1 ? round.ResultTeam1 : round.ResultTeam2)!.Value;
    }

    private static bool IsCounterMatch(TrumpRoundResponse round, JassTeamTyp teamType)
    {
        return teamType == JassTeamTyp.Team1 ? round.IsCounterMatchTeam1 : round.IsCounterMatchTeam2;
    }

    public static bool IsFinished(IReadOnlyList<TrumpRoundResponse> trumpRounds)
    {
        return trumpRounds.Count > 0 &&
               trumpRounds.All(round => round.ResultTeam1.HasValue && round.ResultTeam2.HasValue);
    }

    private static bool IsMatch(TrumpRoundResponse round, JassTeamTyp teamType)
    {
        return teamType == JassTeamTyp.Team1 ? round.IsMatchTeam1 : round.IsMatchTeam2;
    }

    public static bool IsValidTeamStructure(IReadOnlyList<JassTeamResponse> teams)
    {
        return teams.Count == 2 && teams.All(team => team.Players.Count == 2);
    }
}