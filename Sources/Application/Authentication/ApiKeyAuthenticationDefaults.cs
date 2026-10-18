namespace JassApi.Authentication;

public static class ApiKeyAuthenticationDefaults
{
    public const string Scheme = "ApiKey";
    public const string HeaderName = "X-Api-Key";
    public const string ClientName = "api-key-client";
    public const string RequiredScope = "jass.read";
}
