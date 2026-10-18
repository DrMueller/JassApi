using System.Text;
using Microsoft.AspNetCore.Authentication;

namespace JassApi.Authentication;

public static class ApiKeyAuthenticationExtensions
{
    public static IServiceCollection AddApiKeyAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var configuredKeys = new[] { configuration["AppSettings:ApiKey"] }
            .Concat(configuration.GetSection("AppSettings:ApiKeys").Get<string[]>() ?? [])
            .Where(key => !string.IsNullOrWhiteSpace(key))
            .Select(key => Encoding.UTF8.GetBytes(key!))
            .ToArray();

        if (configuredKeys.Length == 0)
            throw new InvalidOperationException(
                "Configure at least one API key using 'AppSettings:ApiKey' or 'AppSettings:ApiKeys'.");

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = ApiKeyAuthenticationDefaults.Scheme;
                options.DefaultAuthenticateScheme = ApiKeyAuthenticationDefaults.Scheme;
                options.DefaultChallengeScheme = ApiKeyAuthenticationDefaults.Scheme;
            })
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                ApiKeyAuthenticationDefaults.Scheme,
                options => options.Keys = configuredKeys);

        services.AddAuthorizationBuilder()
            .AddPolicy(JassApiAuthorization.PolicyName, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scp", ApiKeyAuthenticationDefaults.RequiredScope);
            });

        return services;
    }
}
