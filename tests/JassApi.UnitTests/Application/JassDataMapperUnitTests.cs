using FluentAssertions;
using JassApi.Application;
using JassApi.Persistence.Entities;
using Xunit;

namespace JassApi.UnitTests.Application;

public partial class JassDataMapperUnitTests
{
    [Fact]
    public void MapRound_LocalStoredDate_PreservesLocalWallClock()
    {
        // Arrange
        var localRound = CreateRoundEntity(DateTime.SpecifyKind(_startedAt, DateTimeKind.Local));

        // Act
        var result = JassDataMapper.MapRound(localRound);

        // Assert
        result.StartedAt.DateTime.Should().Be(_startedAt);
        result.StartedAt.Offset.Should().Be(TimeZoneInfo.Local.GetUtcOffset(_startedAt));
    }

    [Fact]
    public void MapRound_PreservesTeamAndTrumpRoundValues()
    {
        // Arrange
        var round = CreateRoundEntity();

        // Act
        var result = JassDataMapper.MapRound(round);

        // Assert
        result.Teams.Should().HaveCount(_expectedTeamCount);
        result.Teams[0].Players.Should().HaveCount(_expectedPlayersPerTeam);
        result.TrumpRounds[0].ResultTeam1.Should().Be(_team1Points);
        result.TrumpRounds[0].ResultTeam2.Should().Be(_team2Points);
        result.TrumpRounds[0].IsMatchTeam1.Should().BeTrue();
        result.TrumpRounds[0].IsCounterMatchTeam2.Should().BeTrue();
        JassDomainCalculations.CalculateTeamPoints(result.TrumpRounds, JassTeamTyp.Team1, false).Should()
            .Be(_expectedTeamPointsIncludingMatches);
        JassDomainCalculations.CalculateMatches(result.TrumpRounds, JassTeamTyp.Team1).Should().Be(_expectedMatchCount);
    }

    [Fact]
    public void MapRound_UnplayedTrumpRound_PreservesNullResults()
    {
        // Arrange
        var round = CreateRoundEntityWithUnplayedTrumpRound();

        // Act
        var result = JassDataMapper.MapRound(round);

        // Assert
        result.TrumpRounds[0].ResultTeam1.Should().BeNull();
        result.TrumpRounds[0].ResultTeam2.Should().BeNull();
        JassDomainCalculations.CalculateTeamPoints(result.TrumpRounds, JassTeamTyp.Team1, false).Should().BeNull();
    }
}