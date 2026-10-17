using JassApi.Persistence.Entities;

namespace JassApi.Application;

public static class JassDataMapper
{
    private static string GetTrumpName(CoiffeurTrumpfTyp trumpType)
    {
        return trumpType switch
        {
            CoiffeurTrumpfTyp.Herz => "Herz",
            CoiffeurTrumpfTyp.Ecken => "Ecken",
            CoiffeurTrumpfTyp.Kreuz => "Kreuz",
            CoiffeurTrumpfTyp.Schaufeln => "Schaufeln",
            CoiffeurTrumpfTyp.Gschobna => "Gschobna",
            CoiffeurTrumpfTyp.Differenzler => "Differenzler",
            CoiffeurTrumpfTyp.Oben => "Oben",
            CoiffeurTrumpfTyp.Unten => "Unten",
            CoiffeurTrumpfTyp.Slalom => "Slalom",
            CoiffeurTrumpfTyp.Misere => "Misere",
            CoiffeurTrumpfTyp.Gustav => "Gustav",
            CoiffeurTrumpfTyp.DrueliDrue => "Drülidrü",
            CoiffeurTrumpfTyp.Tutti => "Tutti",
            CoiffeurTrumpfTyp.Wahl => "Wahl",
            _ => throw new ArgumentOutOfRangeException(nameof(trumpType), trumpType, null)
        };
    }

    public static PlayerResponse MapPlayer(SpielerEntity player)
    {
        return new PlayerResponse(player.Id, player.Name);
    }

    public static PlayerRoundResponse MapRound(CoiffeurSpielrundeEntity round)
    {
        return new PlayerRoundResponse(
            round.Id,
            new DateTimeOffset(DateTime.SpecifyKind(round.GestartetAm, DateTimeKind.Local)),
            round.Punktewert,
            round.IsJassTrainingslager,
            round.JassTeams
                .OrderBy(team => team.JassTeamTyp)
                .Select(team => new JassTeamResponse(
                    team.JassTeamTyp.ToString(),
                    team.JassTeamSpieler
                        .OrderBy(teamPlayer => teamPlayer.Position)
                        .Select(teamPlayer => new JassTeamPlayerResponse(
                            teamPlayer.SpielerId,
                            teamPlayer.Spieler.Name,
                            teamPlayer.Position.ToString(),
                            teamPlayer.IstStartSpieler))
                        .ToArray()))
                .ToArray(),
            round.Trumpfrunden
                .OrderBy(trumpRound => trumpRound.Id)
                .Select(trumpRound => new TrumpRoundResponse(
                    trumpRound.Id,
                    GetTrumpName(trumpRound.CoiffeurTrumpfTyp),
                    trumpRound.PunkteModifikator,
                    trumpRound.ResultatTeam1,
                    trumpRound.ResultatTeam2,
                    trumpRound.IstMatschTeam1,
                    trumpRound.IstMatschTeam2,
                    trumpRound.IstKonterMatchTeam1,
                    trumpRound.IstKonterMatchTeam2))
                .ToArray());
    }
}