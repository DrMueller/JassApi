using System.ComponentModel;
using JassApi.Application;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace JassApi.Mcp;

[McpServerToolType]
public sealed class PlayerDataTools
{
    [McpServerTool(Name = "list_player_rounds", ReadOnly = true, Idempotent = true, UseStructuredContent = true)]
    [Description(
        "Lists completed rounds for a player, optionally filtered by opponent, date range and training-camp status. Includes stored teams, players, results and trump rounds.")]
    public static async Task<IReadOnlyList<PlayerRoundResponse>> ListPlayerRoundsAsync(
        IJassDataQueryService queryService,
        ILogger<PlayerDataTools> logger,
        [Description("Stable ID of the player whose rounds are requested.")]
        int playerId,
        [Description("Optional stable ID of a player on the opposing team.")]
        int? opponentId,
        [Description("Optional inclusive start-time lower bound in ISO 8601 format.")]
        DateTimeOffset? startedAfterOrAt,
        [Description("Optional inclusive start-time upper bound in ISO 8601 format.")]
        DateTimeOffset? startedBeforeOrAt,
        [Description("Optional filter for training-camp rounds.")]
        bool? isTrainingCamp,
        CancellationToken cancellationToken)
    {
        try
        {
            return await queryService.ListPlayerRoundsAsync(
                new PlayerRoundQuery(playerId, opponentId, startedAfterOrAt, startedBeforeOrAt, isTrainingCamp),
                cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "MCP list_player_rounds failed for player {PlayerId}", playerId);
            throw new McpException(McpToolErrorMapper.Map(exception));
        }
    }

    [McpServerTool(Name = "list_players", ReadOnly = true, Idempotent = true, UseStructuredContent = true)]
    [Description("Lists stored Jass players with their stable IDs and names.")]
    public static async Task<IReadOnlyList<PlayerResponse>> ListPlayersAsync(
        IJassDataQueryService queryService,
        ILogger<PlayerDataTools> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            return await queryService.ListPlayersAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "MCP list_players failed");
            throw new McpException(McpToolErrorMapper.Map(exception));
        }
    }
}