using System.Security.Claims;

namespace JassApi.Authentication;

public static class EntraIdPermissionValidator
{
    public static bool HasRequiredPermission(
        ClaimsPrincipal principal,
        IReadOnlyCollection<string> requiredScopes,
        IReadOnlyCollection<string> requiredRoles)
    {
        if (requiredScopes.Count == 0 && requiredRoles.Count == 0) return false;

        var grantedScopes = principal.FindAll("scp")
            .SelectMany(claim => claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        var grantedRoles = principal.FindAll("roles")
            .Concat(principal.FindAll(ClaimTypes.Role))
            .Select(claim => claim.Value);

        return requiredScopes.Intersect(grantedScopes, StringComparer.Ordinal).Any()
               || requiredRoles.Intersect(grantedRoles, StringComparer.Ordinal).Any();
    }
}