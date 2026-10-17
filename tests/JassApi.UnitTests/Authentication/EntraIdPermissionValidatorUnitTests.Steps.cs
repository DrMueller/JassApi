using System.Security.Claims;

namespace JassApi.UnitTests.Authentication;

public partial class EntraIdPermissionValidatorUnitTests
{
    private const string _scopeClaimType = "scp";
    private const string _roleClaimType = "roles";
    private const string _grantedScope = "jass.read other.scope";
    private const string _requiredScope = "jass.read";
    private const string _requiredRole = "Jass.Read.All";

    private static ClaimsPrincipal CreatePrincipal(string claimType, string claimValue)
    {
        return new ClaimsPrincipal(new ClaimsIdentity([new Claim(claimType, claimValue)], "Test"));
    }
}