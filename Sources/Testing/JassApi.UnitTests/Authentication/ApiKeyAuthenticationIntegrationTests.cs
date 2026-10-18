using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace JassApi.UnitTests.Authentication;

public partial class ApiKeyAuthenticationIntegrationTests
{
    [Fact]
    public async Task PostMcp_WithoutApiKey_ReturnsUnauthorizedWithoutChallengeHeader()
    {
        // Arrange
        using var client = _sut.CreateClient(_clientOptions);

        // Act
        using var response = await SendMcpRequestAsync(client, _toolsListRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        response.Headers.WwwAuthenticate.Should().BeEmpty();
    }

    [Fact]
    public async Task PostMcp_WithWrongApiKey_ReturnsUnauthorizedWithoutChallengeHeader()
    {
        // Arrange
        using var client = _sut.CreateClient(_clientOptions);

        // Act
        using var response = await SendMcpRequestAsync(client, _toolsListRequest, _wrongApiKey);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        response.Headers.WwwAuthenticate.Should().BeEmpty();
    }

    [Fact]
    public void CreateHost_WithoutConfiguredApiKey_ThrowsConfigurationException()
    {
        // Arrange
        using var factory = new WebApplicationFactory<Program>();

        // Act
        Action createClient = () => factory.CreateClient(_clientOptions);

        // Assert
        createClient.Should()
            .Throw<InvalidOperationException>()
            .WithMessage(_missingApiKeyConfigurationMessage);
    }

    [Fact]
    public async Task PostMcp_OverHttpWithValidApiKey_RedirectsToHttps()
    {
        // Arrange
        using var client = _sut.CreateClient(_httpClientOptions);

        // Act
        using var response = await SendMcpRequestAsync(client, _toolsListRequest, _apiKey);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.TemporaryRedirect);
        response.Headers.Location!.Scheme.Should().Be("https");
        _queryService.ListPlayersCallCount.Should().Be(0);
        _queryService.ListPlayerRoundsCallCount.Should().Be(0);
    }

    [Fact]
    public async Task PostMcp_WithConfiguredApiKey_InitializesListsToolsAndCallsBothTools()
    {
        // Arrange
        using var client = _sut.CreateClient(_clientOptions);

        // Act
        using var initializeResponse = await SendMcpRequestAsync(client, _initializeRequest, _apiKey);
        using var initializedResponse = await SendMcpRequestAsync(client, _initializedNotification, _apiKey);
        using var toolsListResponse = await SendMcpRequestAsync(client, _toolsListRequest, _rotatedApiKey);
        using var listPlayersResponse = await SendMcpRequestAsync(client, _listPlayersCall, _apiKey);
        using var listPlayerRoundsResponse = await SendMcpRequestAsync(client, _listPlayerRoundsCall, _rotatedApiKey);
        var tools = await ReadToolNamesAsync(toolsListResponse);

        // Assert
        initializeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        initializedResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);
        toolsListResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        listPlayersResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        listPlayerRoundsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        tools.Should().Contain(_listPlayersToolName);
        tools.Should().Contain(_listPlayerRoundsToolName);
        _queryService.ListPlayersCallCount.Should().Be(1);
        _queryService.ListPlayerRoundsCallCount.Should().Be(1);
    }
}
