using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace JassApi.Authentication;

public sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        System.Text.Encodings.Web.UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.IsHttps) return Task.FromResult(AuthenticateResult.Fail("API keys require HTTPS."));
        if (!Request.Headers.TryGetValue(ApiKeyAuthenticationDefaults.HeaderName, out var headerValues))
            return Task.FromResult(AuthenticateResult.NoResult());
        if (headerValues.Count != 1) return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));

        var suppliedKey = Encoding.UTF8.GetBytes(headerValues[0]!);
        var isValid = false;
        foreach (var configuredKey in Options.Keys)
            isValid |= CryptographicOperations.FixedTimeEquals(suppliedKey, configuredKey);

        if (!isValid) return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, ApiKeyAuthenticationDefaults.ClientName),
            new Claim("scp", ApiKeyAuthenticationDefaults.RequiredScope)
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    }
}
