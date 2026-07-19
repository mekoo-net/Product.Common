using System.Text.Json.Serialization;
using Platform.Common.Web;
using MessagePack;

namespace Meeko.Contracts.Keystone;

[MessagePackObject]
public sealed class ListStaffQuery
{
    [Key(0)] public int Page { get; set; } = 1;
    [Key(1)] public int PageSize { get; set; } = 20;
    [Key(2)] public string? Keyword { get; set; }
    [Key(3)] public string? Status { get; set; }
    [Key(4)] public long? RoleId { get; set; }
}

[MessagePackObject]
public sealed class StaffUserDto
{
    [Key(0)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long Uid { get; set; }

    [Key(1)] public string Username { get; set; } = string.Empty;
    [Key(2)] public string Email { get; set; } = string.Empty;
    [Key(3)] public string DisplayName { get; set; } = string.Empty;

    [Key(4)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long RoleId { get; set; }

    [Key(5)] public string RoleName { get; set; } = string.Empty;
    [Key(6)] public string Status { get; set; } = string.Empty;
    [Key(7)] public DateTime? LastLoginAtUtc { get; set; }
    [Key(8)] public string? LastLoginIp { get; set; }
    [Key(9)] public DateTime CreatedAtUtc { get; set; }
    [Key(10)] public DateTime UpdatedAtUtc { get; set; }
}

[MessagePackObject]
public sealed class StaffAdminListResult
{
    [Key(0)] public required StaffUserDto[] Items { get; set; }
    [Key(1)] public required int Total { get; set; }
}

[MessagePackObject]
public sealed class CreateStaffCommand
{
    [Key(0)] public string Username { get; set; } = string.Empty;
    [Key(1)] public string Email { get; set; } = string.Empty;
    [Key(2)] public string DisplayName { get; set; } = string.Empty;
    [Key(3)] public string Password { get; set; } = string.Empty;

    [Key(4)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long RoleId { get; set; }

    [Key(5)]
    [JsonConverter(typeof(NullableLongToStringConverter))]
    public long? OperatorStaffUid { get; set; }
}

[MessagePackObject]
public sealed class UpdateStaffCommand
{
    [Key(0)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long StaffUid { get; set; }

    [Key(1)] public string DisplayName { get; set; } = string.Empty;
    [Key(2)] public string Email { get; set; } = string.Empty;

    [Key(3)]
    [JsonConverter(typeof(NullableLongToStringConverter))]
    public long? OperatorStaffUid { get; set; }
}

[MessagePackObject]
public sealed class SetStaffStatusCommand
{
    [Key(0)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long StaffUid { get; set; }

    [Key(1)] public bool Active { get; set; }

    [Key(2)]
    [JsonConverter(typeof(NullableLongToStringConverter))]
    public long? OperatorStaffUid { get; set; }
}

[MessagePackObject]
public sealed class ResetStaffPasswordCommand
{
    [Key(0)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long StaffUid { get; set; }

    [Key(1)] public string NewPassword { get; set; } = string.Empty;

    [Key(2)]
    [JsonConverter(typeof(NullableLongToStringConverter))]
    public long? OperatorStaffUid { get; set; }
}

[MessagePackObject]
public sealed class ChangeStaffRoleCommand
{
    [Key(0)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long StaffUid { get; set; }

    [Key(1)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long RoleId { get; set; }

    [Key(2)]
    [JsonConverter(typeof(NullableLongToStringConverter))]
    public long? OperatorStaffUid { get; set; }
}

[MessagePackObject]
public sealed class StaffRoleDto
{
    [Key(0)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long Id { get; set; }

    [Key(1)] public string Name { get; set; } = string.Empty;
    [Key(2)] public string? Description { get; set; }
    [Key(3)] public bool IsSystem { get; set; }
    [Key(4)] public string[] PermissionCodes { get; set; } = [];
    [Key(5)] public int MemberCount { get; set; }
    [Key(6)] public DateTime CreatedAtUtc { get; set; }
}

[MessagePackObject]
public sealed class ListStaffRolesQuery
{
    [Key(0)] public int Page { get; set; } = 1;
    [Key(1)] public int PageSize { get; set; } = 20;
    [Key(2)] public string? Keyword { get; set; }
}

/// <summary>角色列表项：只含权限<b>数量</b>，不下发完整权限码集合，编辑时再按 id 拉取明细。</summary>
[MessagePackObject]
public sealed class StaffRoleListItemDto
{
    [Key(0)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long Id { get; set; }

    [Key(1)] public string Name { get; set; } = string.Empty;
    [Key(2)] public string? Description { get; set; }
    [Key(3)] public bool IsSystem { get; set; }
    [Key(4)] public int PermissionCount { get; set; }
    [Key(5)] public int MemberCount { get; set; }
    [Key(6)] public DateTime CreatedAtUtc { get; set; }
}

[MessagePackObject]
public sealed class StaffRoleListResult
{
    [Key(0)] public required StaffRoleListItemDto[] Items { get; set; }
    [Key(1)] public required int Total { get; set; }
}

[MessagePackObject]
public sealed class StaffPermissionDto
{
    [Key(0)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long Id { get; set; }

    [Key(1)] public string Code { get; set; } = string.Empty;
    [Key(2)] public string? Description { get; set; }
}

[MessagePackObject]
public sealed class CreateStaffRoleCommand
{
    [Key(0)] public string Name { get; set; } = string.Empty;
    [Key(1)] public string? Description { get; set; }
    [Key(2)] public string[] PermissionCodes { get; set; } = [];

    [Key(3)]
    [JsonConverter(typeof(NullableLongToStringConverter))]
    public long? OperatorStaffUid { get; set; }
}

[MessagePackObject]
public sealed class UpdateStaffRoleCommand
{
    [Key(0)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long RoleId { get; set; }

    [Key(1)] public string Name { get; set; } = string.Empty;
    [Key(2)] public string? Description { get; set; }
    [Key(3)] public string[] PermissionCodes { get; set; } = [];

    [Key(4)]
    [JsonConverter(typeof(NullableLongToStringConverter))]
    public long? OperatorStaffUid { get; set; }
}

[MessagePackObject]
public sealed class DeleteStaffRoleCommand
{
    [Key(0)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long RoleId { get; set; }

    [Key(1)]
    [JsonConverter(typeof(NullableLongToStringConverter))]
    public long? OperatorStaffUid { get; set; }
}

[MessagePackObject]
public sealed class StaffAdminCommandResult
{
    [Key(0)] public bool Success { get; set; }
    [Key(1)] public StaffUserDto? Staff { get; set; }
    [Key(2)] public StaffRoleDto? Role { get; set; }
    [Key(3)] public string? FailureCode { get; set; }
    [Key(4)] public string? FailureMessage { get; set; }

    public static StaffAdminCommandResult OkStaff(StaffUserDto staff) =>
        new() { Success = true, Staff = staff };

    public static StaffAdminCommandResult OkRole(StaffRoleDto role) =>
        new() { Success = true, Role = role };

    public static StaffAdminCommandResult Ok() =>
        new() { Success = true };

    public static StaffAdminCommandResult Fail(string code, string message) =>
        new() { Success = false, FailureCode = code, FailureMessage = message };
}
