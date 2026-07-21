using Product.Common.Authorization;
using Platform.Common.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using ZiggyCreatures.Caching.Fusion;

namespace Product.Common.Identity;

/// <summary>
/// ???? Staff ??????? X-Staff-Role header ???? Staff ???
/// ?? <see cref="IStaffRolePermissionSource"/> ?? role?permissions ????? FusionCache ???
/// </summary>
public interface IStaffPermissionAuthorizer
{
    Task<bool> HasPermissionAsync(string permissionCode, CancellationToken ct = default);
}

/// <summary>Staff role ? permission codes ?????????????? Keystone gRPC ?????</summary>
public interface IStaffRolePermissionSource
{
    Task<IReadOnlyCollection<string>> GetPermissionsForStaffRoleAsync(string roleName, CancellationToken ct = default);
}

internal sealed class FusionCacheStaffPermissionAuthorizer : IStaffPermissionAuthorizer
{
    private readonly IFusionCache _cache;
    private readonly IStaffRolePermissionSource _source;
    private readonly ICurrentAuth _currentUser;
    private readonly CacheOptions _cacheOptions;
    private readonly ILogger<FusionCacheStaffPermissionAuthorizer> _logger;

    public FusionCacheStaffPermissionAuthorizer(
        IFusionCache cache,
        IStaffRolePermissionSource source,
        ICurrentAuth currentUser,
        CacheOptions cacheOptions,
        ILogger<FusionCacheStaffPermissionAuthorizer> logger)
    {
        _cache = cache;
        _source = source;
        _currentUser = currentUser;
        _cacheOptions = cacheOptions;
        _logger = logger;
    }

    public async Task<bool> HasPermissionAsync(string permissionCode, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(permissionCode)) return false;
        if (!_currentUser.IsStaff) return false;
        var staffRole = _currentUser.StaffRole;
        if (string.IsNullOrWhiteSpace(staffRole)) return false;

        var perms = await GetPermissionsAsync(staffRole.Trim(), ct);
        return perms.Contains(permissionCode, StringComparer.OrdinalIgnoreCase);
    }

    private async Task<IReadOnlyCollection<string>> GetPermissionsAsync(string roleName, CancellationToken ct)
    {
        var key = $"meeko:rbac:staff-role-perms:{roleName.ToLowerInvariant()}";
        return await _cache.GetOrSetAsync<IReadOnlyCollection<string>>(
            key,
            async (_, innerCt) =>
            {
                try
                {
                    return await _source.GetPermissionsForStaffRoleAsync(roleName, innerCt);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to load staff role permissions for {Role}; falling back to empty set (not cached).", roleName);
                    throw;
                }
            },
            failSafeDefaultValue: Array.Empty<string>(),
            setupAction: opt => opt.ApplyProfile(_cacheOptions, _cacheOptions.Profiles.RbacStaff),
            token: ct);
    }
}

public static class StaffPermissionAuthorizerExtensions
{
    /// <summary>
    /// ?? Staff ????? + permission authorization handler?
    /// ??????? <c>AddPlatformCache</c>???????? <see cref="IStaffRolePermissionSource"/>?
    /// </summary>
    public static IServiceCollection AddStaffPermissions(this IServiceCollection services)
    {
        services.TryAddScoped<IStaffPermissionAuthorizer, FusionCacheStaffPermissionAuthorizer>();
        services.AddHttpContextAccessor();
        services.AddPermissionAuthorization();
        return services;
    }
}
