namespace Product.Common.Identity;

/// <summary>
/// 下游服务对当前用户的视图。值由 Gateway 注入的 header 构建，禁止再次校验 JWT。
/// </summary>
public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    string? UserId { get; }

    /// <summary>X-Iam-Uid：当前登录子账号（IamUser）的对外 Uid，仅用于归属审计；
    /// 仅 <see cref="Actor"/> == "user" 时有值（apikey/staff 路径无）。鉴权仍以账户为准。</summary>
    string? IamUserUid { get; }

    string? TraceId { get; }
    IReadOnlyCollection<string> Roles { get; }

    /// <summary>X-Actor：staff / user / apikey，对应 Keystone 双 issuer 结构（§KS-ADR-3/5）。</summary>
    string? Actor { get; }

    /// <summary>X-Staff-Role：仅当 <see cref="Actor"/> == "staff" 时有值。</summary>
    string? StaffRole { get; }

    bool IsInRole(string role);

    bool IsStaff => string.Equals(Actor, "staff", StringComparison.OrdinalIgnoreCase);
}
