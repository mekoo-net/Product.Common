using Platform.Common.Identity;
using Product.Common.PlatformServices;

namespace Product.Common.Auth.Staff;

/// <summary>Staff 角色 → permission 码（经 Keystone Query RPC）。</summary>
public sealed class PlatformStaffPermissionSource : IStaffRolePermissionSource
{
    private readonly KeystonePlatformService _keystone;

    public PlatformStaffPermissionSource(KeystonePlatformService keystone) => _keystone = keystone;

    public async Task<IReadOnlyCollection<string>> GetPermissionsForStaffRoleAsync(string roleName, CancellationToken ct = default)
    {
        var result = await _keystone.Query.GetStaffRolePermissionsAsync(roleName);
        return result.Permissions ?? [];
    }
}
