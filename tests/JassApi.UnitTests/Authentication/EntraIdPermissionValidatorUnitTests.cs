using System.Security.Claims;
using FluentAssertions;
using JassApi.Authentication;
using Xunit;

namespace JassApi.UnitTests.Authentication;

public partial class EntraIdPermissionValidatorUnitTests
{
    [Fact]
    public void HasRequiredPermission_GrantedApplicationRoleInMappedClaim_ReturnsTrue()
    {
        // Arrange
        var principal = CreatePrincipal(ClaimTypes.Role, _requiredRole);

        // Act
        var hasPermission = EntraIdPermissionValidator.HasRequiredPermission(
            principal,
            [],
            [_requiredRole]);

        // Assert
        hasPermission.Should().BeTrue();
    }

    [Fact]
    public void HasRequiredPermission_GrantedApplicationRole_ReturnsTrue()
    {
        // Arrange
        var principal = CreatePrincipal(_roleClaimType, _requiredRole);

        // Act
        var hasPermission = EntraIdPermissionValidator.HasRequiredPermission(
            principal,
            [],
            [_requiredRole]);

        // Assert
        hasPermission.Should().BeTrue();
    }

    [Fact]
    public void HasRequiredPermission_GrantedDelegatedScope_ReturnsTrue()
    {
        // Arrange
        var principal = CreatePrincipal(_scopeClaimType, _grantedScope);

        // Act
        var hasPermission = EntraIdPermissionValidator.HasRequiredPermission(
            principal,
            [_requiredScope],
            []);

        // Assert
        hasPermission.Should().BeTrue();
    }

    [Fact]
    public void HasRequiredPermission_NoConfiguredPermission_ReturnsFalse()
    {
        // Arrange
        var principal = CreatePrincipal(_scopeClaimType, _grantedScope);

        // Act
        var hasPermission = EntraIdPermissionValidator.HasRequiredPermission(principal, [], []);

        // Assert
        hasPermission.Should().BeFalse();
    }
}