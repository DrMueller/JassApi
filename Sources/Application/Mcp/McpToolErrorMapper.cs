namespace JassApi.Mcp;

public static class McpToolErrorMapper
{
    public const string GenericErrorMessage = "The Jass data request could not be completed. Please retry later.";

    public static string Map(Exception exception)
    {
        return exception switch
        {
            ArgumentException => exception.Message,
            KeyNotFoundException => exception.Message,
            _ => GenericErrorMessage
        };
    }
}