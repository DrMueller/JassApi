using JassApi.Persistence.Entities;

namespace JassApi.UnitTests.Application;

public partial class JassDataMapperUnitTests
{
    private const int _expectedTeamCount = 2;
    private const int _expectedPlayersPerTeam = 2;
    private const int _roundId = 41;
    private const int _trumpRoundId = 77;
    private const int _team1Points = 157;
    private const int _team2Points = 3;
    private const int _expectedMatchCount = 4;
    private const int _expectedTeamPointsIncludingMatches = 388;
    private static readonly DateTime _startedAt = new(2026, 8, 12, 18, 30, 0, DateTimeKind.Utc);

    private static CoiffeurSpielrundeEntity CreateRoundEntity(DateTime? startedAt = null)
    {
        var round = new CoiffeurSpielrundeEntity
        {
            Id = _roundId,
            GestartetAm = startedAt ?? _startedAt,
            Punktewert = 10,
            IsJassTrainingslager = false,
            Trumpfrunden =
            [
                new TrumpfrundeEntity
                {
                    Id = _trumpRoundId,
                    CoiffeurTrumpfTyp = CoiffeurTrumpfTyp.Herz,
                    PunkteModifikator = 2,
                    ResultatTeam1 = _team1Points,
                    ResultatTeam2 = _team2Points,
                    IstMatschTeam1 = true,
                    IstKonterMatchTeam2 = true
                }
            ],
            JassTeams = CreateTeams()
        };

        return round;
    }

    private static CoiffeurSpielrundeEntity CreateRoundEntityWithUnplayedTrumpRound()
    {
        var round = CreateRoundEntity();
        round.Trumpfrunden.Single().ResultatTeam1 = null;
        round.Trumpfrunden.Single().ResultatTeam2 = null;

        return round;
    }

    private static JassTeamEntity CreateTeam(
        JassTeamTyp teamType,
        int firstPlayerId,
        string firstPlayerName,
        int secondPlayerId,
        string secondPlayerName)
    {
        return new JassTeamEntity
        {
            Id = firstPlayerId,
            JassTeamTyp = teamType,
            JassTeamSpieler =
            [
                new JassTeamSpielerEntity
                {
                    Id = firstPlayerId,
                    SpielerId = firstPlayerId,
                    Position = JassTeamSpielerPosition.Spieler1,
                    IstStartSpieler = true,
                    Spieler = new SpielerEntity { Id = firstPlayerId, Name = firstPlayerName }
                },
                new JassTeamSpielerEntity
                {
                    Id = secondPlayerId,
                    SpielerId = secondPlayerId,
                    Position = JassTeamSpielerPosition.Spieler2,
                    IstStartSpieler = false,
                    Spieler = new SpielerEntity { Id = secondPlayerId, Name = secondPlayerName }
                }
            ]
        };
    }

    private static List<JassTeamEntity> CreateTeams()
    {
        return
        [
            CreateTeam(JassTeamTyp.Team1, 11, "Anna", 12, "Ben"),
            CreateTeam(JassTeamTyp.Team2, 13, "Clara", 14, "Dario")
        ];
    }
}