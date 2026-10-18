using JassApi.Application;
using JassApi.Persistence;
using JassApi.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace JassApi.UnitTests.Application;

public partial class JassDataQueryServiceUnitTests
{
    private const int _playerId = 23;
    private const int _opponentId = 29;
    private const int _expectedTeamCount = 2;
    private const int _roundId = 37;
    private const int _trumpRoundId = 43;
    private const int _partnerId = 31;
    private const int _opponentPartnerId = 41;
    private const int _expectedCreateCountBeforeCall = 0;
    private const string _playerName = "Founi";
    private const string _partnerName = "Partner";
    private const string _opponentName = "Leiser";
    private const string _opponentPartnerName = "Opponent partner";
    private static readonly DateTimeOffset _earlierDate = new(2026, 3, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset _laterDate = new(2026, 4, 1, 0, 0, 0, TimeSpan.Zero);
    private readonly IJassApiDbContextFactory _appDbContextFactory;
    private readonly JassDataQueryService _sut;

    public JassDataQueryServiceUnitTests()
    {
        var options = new DbContextOptionsBuilder<JassApiDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _appDbContextFactory = new JassApiDbContextFactory(options);
        _sut = new JassDataQueryService(_appDbContextFactory);
    }

    private static CoiffeurSpielrundeEntity CreateRoundWithPlayersInSameTeam()
    {
        return new CoiffeurSpielrundeEntity
        {
            Id = _roundId,
            GestartetAm = _earlierDate.LocalDateTime,
            Punktewert = 10,
            JassTeams =
            [
                CreateTeam(JassTeamTyp.Team1, _playerId, _playerName, _opponentId, _opponentName),
                CreateTeam(JassTeamTyp.Team2, _partnerId, _partnerName, _opponentPartnerId, _opponentPartnerName)
            ],
            Trumpfrunden =
            [
                new TrumpfrundeEntity
                {
                    Id = _trumpRoundId,
                    CoiffeurSpielrundeId = _roundId,
                    CoiffeurTrumpfTyp = CoiffeurTrumpfTyp.Misere,
                    PunkteModifikator = 9,
                    ResultatTeam1 = 9,
                    ResultatTeam2 = 7
                }
            ]
        };
    }

    private static CoiffeurSpielrundeEntity CreateRoundWithTeams()
    {
        return new CoiffeurSpielrundeEntity
        {
            Id = _roundId,
            GestartetAm = _earlierDate.LocalDateTime,
            Punktewert = 10,
            JassTeams =
            [
                CreateTeam(JassTeamTyp.Team1, _playerId, _playerName, _partnerId, _partnerName),
                CreateTeam(JassTeamTyp.Team2, _opponentId, _opponentName, _opponentPartnerId, _opponentPartnerName)
            ],
            Trumpfrunden =
            [
                new TrumpfrundeEntity
                {
                    Id = _trumpRoundId,
                    CoiffeurSpielrundeId = _roundId,
                    CoiffeurTrumpfTyp = CoiffeurTrumpfTyp.Misere,
                    PunkteModifikator = 9,
                    ResultatTeam1 = 9,
                    ResultatTeam2 = 7
                }
            ]
        };
    }

    private static JassTeamEntity CreateTeam(
        JassTeamTyp teamType,
        int playerId,
        string playerName,
        int partnerId,
        string partnerName)
    {
        return new JassTeamEntity
        {
            CoiffeurSpielrundeId = _roundId,
            JassTeamTyp = teamType,
            JassTeamSpieler =
            [
                CreateTeamPlayer(playerId, teamType, playerName, JassTeamSpielerPosition.Spieler1),
                CreateTeamPlayer(partnerId, teamType, partnerName, JassTeamSpielerPosition.Spieler2)
            ]
        };
    }

    private static JassTeamSpielerEntity CreateTeamPlayer(
        int playerId,
        JassTeamTyp teamType,
        string playerName,
        JassTeamSpielerPosition position)
    {
        return new JassTeamSpielerEntity
        {
            SpielerId = playerId,
            Position = position,
            IstStartSpieler = position == JassTeamSpielerPosition.Spieler1,
            Spieler = new SpielerEntity { Id = playerId, Name = playerName }
        };
    }

    private async Task SeedRoundWithPlayersInSameTeamAsync()
    {
        await using var context = _appDbContextFactory.Create();
        var round = CreateRoundWithPlayersInSameTeam();
        context.Rounds.Add(round);
        await context.SaveChangesAsync();
    }

    private async Task SeedRoundWithTeamsAsync()
    {
        await using var context = _appDbContextFactory.Create();
        var round = CreateRoundWithTeams();
        context.Rounds.Add(round);
        await context.SaveChangesAsync();
    }
}