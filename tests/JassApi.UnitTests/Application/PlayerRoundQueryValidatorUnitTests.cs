using FluentAssertions;
using JassApi.Application;
using Xunit;

namespace JassApi.UnitTests.Application;

public partial class PlayerRoundQueryValidatorUnitTests
{
    [Fact]
    public void Validate_EqualDateBounds_ReturnsNoErrors()
    {
        // Arrange
        var query = CreateQuery(startedAfterOrAt: _earlierDate, startedBeforeOrAt: _earlierDate);

        // Act
        var errors = PlayerRoundQueryValidator.Validate(query);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_PlayerAndOpponentAreSame_ReturnsValidationError()
    {
        // Arrange
        var query = CreateQuery(_playerId, _playerId);

        // Act
        var errors = PlayerRoundQueryValidator.Validate(query);

        // Assert
        errors.Should().ContainSingle().Which.Should().Be(PlayerRoundQueryValidator.SamePlayerAndOpponentMessage);
    }

    [Fact]
    public void Validate_StartDateAfterEndDate_ReturnsValidationError()
    {
        // Arrange
        var query = CreateQuery(startedAfterOrAt: _laterDate, startedBeforeOrAt: _earlierDate);

        // Act
        var errors = PlayerRoundQueryValidator.Validate(query);

        // Assert
        errors.Should().ContainSingle().Which.Should().Be(PlayerRoundQueryValidator.InvalidDateRangeMessage);
    }
}