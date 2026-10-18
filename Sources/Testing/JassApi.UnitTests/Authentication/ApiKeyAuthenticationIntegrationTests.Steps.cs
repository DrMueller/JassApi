using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using JassApi.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JassApi.UnitTests.Authentication;

public partial class ApiKeyAuthenticationIntegrationTests : IDisposable
{
    private const string _apiKey = "integration-test-api-key";
    private const string _rotatedApiKey = "integration-test-rotated-api-key";
    private const string _wrongApiKey = "integration-test-wrong-api-key";
    private const string _apiKeyHeader = "X-Api-Key";

    private const string _missingApiKeyConfigurationMessage =
        "Configure at least one API key using 'AppSettings:ApiKey' or 'AppSettings:ApiKeys'.";

    private const string _listPlayersToolName = "list_players";
    private const string _listPlayerRoundsToolName = "list_player_rounds";
    private const int _playerId = 42;

    private const string _initializeRequest = """
                                              {"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2024-11-05","capabilities":{},"clientInfo":{"name":"JassApi.UnitTests","version":"1.0.0"}}}
                                              """;

    private const string _initializedNotification = """
                                                    {"jsonrpc":"2.0","method":"notifications/initialized"}
                                                    """;

    private const string _toolsListRequest = """
                                             {"jsonrpc":"2.0","id":2,"method":"tools/list","params":{}}
                                             """;

    private static readonly string _listPlayersCall = JsonSerializer.Serialize(new
    {
        jsonrpc = "2.0",
        id = 3,
        method = "tools/call",
        @params = new { name = _listPlayersToolName, arguments = new { } }
    });

    private static readonly string _listPlayerRoundsCall = JsonSerializer.Serialize(new
    {
        jsonrpc = "2.0",
        id = 4,
        method = "tools/call",
        @params = new
        {
            name = _listPlayerRoundsToolName,
            arguments = new
            {
                playerId = _playerId,
                opponentId = (int?)null,
                startedAfterOrAt = (DateTimeOffset?)null,
                startedBeforeOrAt = (DateTimeOffset?)null,
                isTrainingCamp = (bool?)null
            }
        }
    });

    private static readonly WebApplicationFactoryClientOptions _clientOptions = new()
    {
        BaseAddress = new Uri("https://localhost"),
        AllowAutoRedirect = false
    };

    private static readonly WebApplicationFactoryClientOptions _httpClientOptions = new()
    {
        BaseAddress = new Uri("http://localhost"),
        AllowAutoRedirect = false
    };

    private readonly TestJassDataQueryService _queryService = new();

    private readonly WebApplicationFactory<Program> _sut;

    public ApiKeyAuthenticationIntegrationTests()
    {
        _sut = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting("https_port", "443");
            builder.UseSetting("AppSettings:ApiKey", _apiKey);
            builder.UseSetting("AppSettings:ApiKeys:0", _rotatedApiKey);
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IJassDataQueryService>();
                services.AddSingleton<IJassDataQueryService>(_queryService);
            });
        });
    }

    public void Dispose()
    {
        _sut.Dispose();
    }

    private static async Task<JsonDocument> ReadJsonResponseAsync(HttpResponseMessage response)
    {
        var responseBody = await response.Content.ReadAsStringAsync();
        var eventData = responseBody
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault(line => line.StartsWith("data: ", StringComparison.Ordinal));
        var json = eventData is null ? responseBody : eventData[6..];

        return JsonDocument.Parse(json);
    }

    private static async Task<string?[]> ReadToolNamesAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode) return [];

        using var document = await ReadJsonResponseAsync(response);
        return document.RootElement
            .GetProperty("result")
            .GetProperty("tools")
            .EnumerateArray()
            .Select(tool => tool.GetProperty("name").GetString())
            .ToArray();
    }

    private static async Task<HttpResponseMessage> SendMcpRequestAsync(
        HttpClient client,
        string body,
        string? apiKey = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/mcp")
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));

        if (apiKey is not null) request.Headers.Add(_apiKeyHeader, apiKey);

        return await client.SendAsync(request);
    }

    private sealed class TestJassDataQueryService : IJassDataQueryService
    {
        public int ListPlayerRoundsCallCount { get; private set; }
        public int ListPlayersCallCount { get; private set; }

        public Task<IReadOnlyList<PlayerRoundResponse>> ListPlayerRoundsAsync(
            PlayerRoundQuery query,
            CancellationToken cancellationToken)
        {
            ListPlayerRoundsCallCount++;
            return Task.FromResult<IReadOnlyList<PlayerRoundResponse>>([]);
        }

        public Task<IReadOnlyList<PlayerResponse>> ListPlayersAsync(CancellationToken cancellationToken)
        {
            ListPlayersCallCount++;
            return Task.FromResult<IReadOnlyList<PlayerResponse>>([new PlayerResponse(_playerId, "Test Player")]);
        }
    }
}