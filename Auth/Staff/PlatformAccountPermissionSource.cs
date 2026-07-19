using Platform.Common.Identity;
using Product.Common.PlatformServices;

namespace Product.Common.Auth.Staff;

/// <summary>Account 角色 → permission 码（经 Keystone Query RPC）。</summary>
public sealed class PlatformAccountPermissionSource : IAccountRolePermissionSource
{
    private readonly KeystonePlatformService _keystone;

    public PlatformAccountPermissionSource(KeystonePlatformService keystone) => _keystone = keystone;

    public async Task<IReadOnlyCollection<string>> GetPermissionsForAccountRoleAsync(string roleName, CancellationToken ct = default)
    {
        var result = await _keystone.Query.GetRolePermissionsAsync(roleName);
        return result.Permissions ?? [];
    }
}
