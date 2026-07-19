using MessagePack;

namespace Meeko.Contracts.Keystone;

/// <summary>单条权限注册项（产品 → 平台）。</summary>
[MessagePackObject]
public sealed class PermissionRegistrationItem
{
    /// <summary>权限码，如 <c>demux:models:read</c>。</summary>
    [Key(0)] public string Code { get; set; } = string.Empty;

    /// <summary>人类可读描述（控制台角色页直接展示）。</summary>
    [Key(1)] public string? Description { get; set; }

    /// <summary>只读操作（查看类）；ReadOnly 系统角色自动获得该码。</summary>
    [Key(2)] public bool ReadOnly { get; set; }
}

/// <summary>产品权限目录注册命令（幂等：重复调用只补缺失的码/授权、刷新描述）。</summary>
[MessagePackObject]
public sealed class RegisterPermissionCatalogCommand
{
    /// <summary>产品标识（如 <c>demux</c>），用于日志与审计。</summary>
    [Key(0)] public string ProductKey { get; set; } = string.Empty;

    [Key(1)] public PermissionRegistrationItem[] Items { get; set; } = [];
}

[MessagePackObject]
public sealed class RegisterPermissionCatalogResult
{
    [Key(0)] public bool Success { get; set; }
    [Key(1)] public int PermissionsAdded { get; set; }
    [Key(2)] public int GrantsAdded { get; set; }
    [Key(3)] public string? FailureMessage { get; set; }

    public static RegisterPermissionCatalogResult Ok(int permissionsAdded, int grantsAdded) =>
        new() { Success = true, PermissionsAdded = permissionsAdded, GrantsAdded = grantsAdded };

    public static RegisterPermissionCatalogResult Fail(string message) =>
        new() { Success = false, FailureMessage = message };
}
