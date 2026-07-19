using MessagePack;

namespace Meeko.Contracts.Keystone;

[MessagePackObject]
public sealed class RolePermissions
{
    [Key(0)] public required string RoleName { get; set; }
    [Key(1)] public required string[] Permissions { get; set; }
}
