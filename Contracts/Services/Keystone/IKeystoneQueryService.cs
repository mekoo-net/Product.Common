using MagicOnion;

namespace Meeko.Contracts.Keystone;

/// <summary>
/// 业务服务（Bff/Billing/Notice/Jobs）→ Keystone：Account 状态、角色权限、子账号列表查询。
/// 见 Meeko-Keystone.md §10.1。所有方法都会被 FusionCache 包裹，命中时不会真正打到 Keystone。
/// </summary>
public interface IKeystoneQueryService : IService<IKeystoneQueryService>
{
    UnaryResult<AccountInfo?> GetAccountAsync(long accountUid);

    UnaryResult<RolePermissions> GetRolePermissionsAsync(string roleName);

    /// <summary>
    /// 查询 Staff 平台角色对应的权限码集合（如 SuperAdmin / ReadOnly）。
    /// 下游服务（Notice/Bff/Billing）通过 FusionCache 缓存返回值，命中时不会真正打到 Keystone。
    /// </summary>
    UnaryResult<RolePermissions> GetStaffRolePermissionsAsync(string roleName);

    UnaryResult<bool> IsAccountActiveAsync(long accountUid);

    /// <summary>仅 Bff 子账号管理页用（Org Owner / Admin）。</summary>
    UnaryResult<List<IamUserInfo>> ListIamUsersAsync(long accountUid);

    /// <summary>仅 Keystone 自己审计反查用 —— 业务服务请勿调用。</summary>
    UnaryResult<IamUserInfo?> GetIamUserBySessionAsync(long sessionUid);
}
