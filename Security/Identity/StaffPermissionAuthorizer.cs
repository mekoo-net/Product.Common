using Product.Common.Authorization;
using Platform.Common.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using ZiggyCreatures.Caching.Fusion;

namespace Product.Common.Identity;

/// <summary>
/// 跨服务的 Staff 权限校验。基�?X-Staff-Role header 解析当前 Staff 角色�?
/// 通过 <see cref="IStaffRolePermissionSource"/> 拉取 role→permissions 映射，并�?FusionCache 缓存�?
/// </summary>
public interface IStaffPermissionAuthorizer
{
    Task<bool> HasPermissionAsync(string permissionCode, CancellationToken ct = default);

    Task<bool> HasPermissionAsync(string? actor, string? staffRole, string permissionCode, CancellationToken ct = default);
}

/// <summary>Staff role �?permission codes 数据源（由调用方注册，通常�?Keystone gRPC 客户端）�?/summary>
public interface IStaffRolePermissionSource
{
    Task<IReadOnlyCollection<string>> GetPermissionsForStaffRoleAsync(string roleName, CancellationToken ct = default);
}

internal sealed class FusionCacheStaffPermissionAuthorizer : IStaffPermissionAuthorizer
{
    private const string StaffActor = "staff";

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

    public Task<bool> HasPermissionAsync(string permissionCode, CancellationToken ct = default)
        => HasPermissionAsync(_currentUser.Actor, _currentUser.StaffRole, permissionCode, ct);

    public async Task<bool> HasPermissionAsync(string? actor, string? staffRole, string permissionCode, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(permissionCode)) return false;
        if (!string.Equals(actor, StaffActor, StringComparison.OrdinalIgnoreCase)) return false;
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
    /// 注册 Staff 权限校验�?+ permission authorization handler�?
    /// 要求事先已调�?<c>AddPlatformCache</c>，并由调用方注册 <see cref="IStaffRolePermissionSource"/>�?
    /// </summary>
    public static IServiceCollection AddStaffPermissions(this IServiceCollection services)
    {
        services.TryAddScoped<IStaffPermissionAuthorizer, FusionCacheStaffPermissionAuthorizer>();
        services.AddHttpContextAccessor();
        services.AddPermissionAuthorization();
        return services;
    }
}
