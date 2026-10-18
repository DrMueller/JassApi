using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using ModelContextProtocol.AspNetCore.Authentication;
using ModelContextProtocol.Authentication;

namespace JassApi.Authentication;

public static class EntraIdAuthenticationExtensions
{
    public static IServiceCollection AddEntraIdApiAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var authenticationBuilder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
        authenticationBuilder.AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"));
        authenticationBuilder.AddMcp(options =>
        {
            var instance = configuration["AzureAd:Instance"] ?? "https://login.microsoftonline.com/";
            var tenantId = configuration["AzureAd:TenantId"];
            var resourceUrl = configuration["Mcp:ResourceUrl"];
            var appIdUri = configuration["AzureAd:AppIdUri"]; // api://d80a0b90-3b3e-45a2-804f-9c129a25999c
            var scopes = configuration.GetSection("AzureAd:RequiredScopes").Get<string[]>() ?? [];

            if (!string.IsNullOrWhiteSpace(tenantId)
                && !string.IsNullOrWhiteSpace(resourceUrl)
                && !string.IsNullOrWhiteSpace(appIdUri)
                && Uri.TryCreate(instance, UriKind.Absolute, out _))
                options.ResourceMetadata = new ProtectedResourceMetadata
                {
                    Resource = resourceUrl,
                    AuthorizationServers = { $"{instance.TrimEnd('/')}/{tenantId}/v2.0" },
                    ScopesSupported = scopes.Select(s => $"{appIdUri.TrimEnd('/')}/{s}").ToList()
                };
        });

        services.Configure<AuthenticationOptions>(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = McpAuthenticationDefaults.AuthenticationScheme;
        });

        services.AddAuthorizationBuilder()
            .AddPolicy(JassApiAuthorization.PolicyName, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context => EntraIdPermissionValidator.HasRequiredPermission(
                    context.User,
                    configuration.GetSection("AzureAd:RequiredScopes").Get<string[]>() ?? [],
                    configuration.GetSection("AzureAd:RequiredRoles").Get<string[]>() ?? []));
            });

        return services;
    }
}
