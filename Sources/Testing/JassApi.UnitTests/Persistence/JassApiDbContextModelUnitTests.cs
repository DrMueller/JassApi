using FluentAssertions;
using JassApi.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace JassApi.UnitTests.Persistence;

public partial class JassApiDbContextModelUnitTests
{
    [Fact]
    public void Model_UsesExistingSqlTableNames()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var tableNames = context.Model.GetEntityTypes()
            .Select(entityType => entityType.GetTableName())
            .ToArray();

        // Assert
        tableNames.Should().Contain("SpielerTable");
        tableNames.Should().Contain("CoiffeurSpielrundeTable");
        tableNames.Should().Contain("JassTeamTable");
        tableNames.Should().Contain("JassTeamSpielerTable");
        tableNames.Should().Contain("TrumpfrundeTable");
    }

    [Fact]
    public void Model_UsesIntegerProviderTypeForTrumpRoundResults()
    {
        // Arrange
        using var context = CreateContext();
        var resultProperty = context.Model.FindEntityType(typeof(TrumpfrundeEntity))!
            .FindProperty(nameof(TrumpfrundeEntity.ResultatTeam1))!;

        // Act
        var providerType = resultProperty.GetProviderClrType();

        // Assert
        providerType.Should().BeNull();
        resultProperty.ClrType.Should().Be(typeof(int?));
    }
}