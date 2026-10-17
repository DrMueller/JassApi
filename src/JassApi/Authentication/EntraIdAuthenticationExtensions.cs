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
            var audience = configuration["AzureAd:Audience"];
            var scopes = configuration.GetSection("AzureAd:RequiredScopes").Get<string[]>() ?? [];

            if (!string.IsNullOrWhiteSpace(tenantId)
                && !string.IsNullOrWhiteSpace(audience)
                && Uri.TryCreate(instance, UriKind.Absolute, out _))
                options.ResourceMetadata = new ProtectedResourceMetadata
                {
                    Resource = audience,
                    AuthorizationServers = { $"{instance.TrimEnd('/')}/{tenantId}/v2.0" },
                    ScopesSupported = scopes
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
