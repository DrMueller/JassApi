using Microsoft.AspNetCore.Authentication;

namespace JassApi.Authentication;

public sealed class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public IReadOnlyCollection<byte[]> Keys { get; set; } = Array.Empty<byte[]>();
}
