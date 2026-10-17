using FluentAssertions;
using JassApi.Mcp;
using Xunit;

namespace JassApi.UnitTests.Mcp;

public partial class McpToolErrorMapperUnitTests
{
    [Fact]
    public void Map_ArgumentException_ReturnsValidationMessage()
    {
        // Arrange
        var exception = new ArgumentException(_validationMessage);

        // Act
        var result = McpToolErrorMapper.Map(exception);

        // Assert
        result.Should().Be(_validationMessage);
    }

    [Fact]
    public void Map_KeyNotFoundException_ReturnsNotFoundMessage()
    {
        // Arrange
        var exception = new KeyNotFoundException(_notFoundMessage);

        // Act
        var result = McpToolErrorMapper.Map(exception);

        // Assert
        result.Should().Be(_notFoundMessage);
    }

    [Fact]
    public void Map_UnexpectedException_DoesNotExposeInternalMessage()
    {
        // Arrange
        var exception = new InvalidOperationException(_internalErrorMessage);

        // Act
        var result = McpToolErrorMapper.Map(exception);

        // Assert
        result.Should().Be(_genericErrorMessage);
        result.Should().NotContain(_internalErrorMessage);
    }
}