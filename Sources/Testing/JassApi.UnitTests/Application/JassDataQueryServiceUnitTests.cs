using FluentAssertions;
using JassApi.Application;
using Xunit;

namespace JassApi.UnitTests.Application;

public partial class JassDataQueryServiceUnitTests
{
    [Fact]
    public async Task ListPlayerRoundsAsync_InvalidDateRange_ThrowsBeforeQuerying()
    {
        // Arrange
        var query = new PlayerRoundQuery(_playerId, null, _laterDate, _earlierDate, null);

        // Act
        Func<Task> action = () => _sut.ListPlayerRoundsAsync(query, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
        _appDbContextFactory.CreateCount.Should().Be(_expectedCreateCountBeforeCall);
    }

    [Fact]
    public async Task ListPlayerRoundsAsync_OpponentInSameTeam_ExcludesRound()
    {
        // Arrange
        await SeedRoundWithPlayersInSameTeamAsync();
        var query = new PlayerRoundQuery(_playerId, _opponentId, null, null, null);

        // Act
        var rounds = await _sut.ListPlayerRoundsAsync(query, CancellationToken.None);

        // Assert
        rounds.Should().BeEmpty();
    }

    [Fact]
    public async Task ListPlayerRoundsAsync_RelatedPlayersStored_ReturnsTeamPlayers()
    {
        // Arrange
        await SeedRoundWithTeamsAsync();
        var query = new PlayerRoundQuery(_playerId, _opponentId, null, null, null);

        // Act
        var rounds = await _sut.ListPlayerRoundsAsync(query, CancellationToken.None);

        // Assert
        rounds.Should().ContainSingle();
        rounds[0].Teams.Should().HaveCount(_expectedTeamCount);
        rounds[0].Teams.SelectMany(team => team.Players).Select(player => player.Name)
            .Should().Contain([_playerName, _partnerName, _opponentName, _opponentPartnerName]);
    }
}