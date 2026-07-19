using MagicOnion;

namespace Meeko.Contracts.Keystone;

/// <summary>Bff → Keystone：平台 Staff 账号与角色管理。</summary>
public interface IKeystoneStaffAdminService : IService<IKeystoneStaffAdminService>
{
    UnaryResult<StaffAdminListResult> ListStaffAsync(ListStaffQuery query);
    UnaryResult<StaffUserDto?> GetStaffAsync(long staffUid);

    UnaryResult<StaffAdminCommandResult> CreateStaffAsync(CreateStaffCommand cmd);
    UnaryResult<StaffAdminCommandResult> UpdateStaffAsync(UpdateStaffCommand cmd);
    UnaryResult<StaffAdminCommandResult> SetStaffStatusAsync(SetStaffStatusCommand cmd);
    UnaryResult<StaffAdminCommandResult> ResetStaffPasswordAsync(ResetStaffPasswordCommand cmd);
    UnaryResult<StaffAdminCommandResult> ChangeStaffRoleAsync(ChangeStaffRoleCommand cmd);

    UnaryResult<StaffRoleListResult> ListRolesAsync(ListStaffRolesQuery query);
    UnaryResult<StaffRoleDto?> GetRoleAsync(long roleId);
    UnaryResult<StaffAdminCommandResult> CreateRoleAsync(CreateStaffRoleCommand cmd);
    UnaryResult<StaffAdminCommandResult> UpdateRoleAsync(UpdateStaffRoleCommand cmd);
    UnaryResult<StaffAdminCommandResult> DeleteRoleAsync(DeleteStaffRoleCommand cmd);

    UnaryResult<StaffPermissionDto[]> ListPermissionCatalogAsync();

    /// <summary>
    /// 产品服务（Demux/Storage/…）启动时自注册权限目录（幂等 upsert）。
    /// 平台只存码 + 描述，不引用产品的权限常量 —— 产品域权限与 Keystone 彻底解耦。
    /// 新码自动授予系统角色：SuperAdmin 获全部，ReadOnly 获 <c>readOnly=true</c> 子集。
    /// </summary>
    UnaryResult<RegisterPermissionCatalogResult> RegisterPermissionCatalogAsync(RegisterPermissionCatalogCommand cmd);
}
