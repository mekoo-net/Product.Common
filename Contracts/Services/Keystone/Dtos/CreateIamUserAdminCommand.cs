using MessagePack;

namespace Meeko.Contracts.Keystone;

[MessagePackObject]
public sealed class CreateIamUserAdminCommand
{
    [Key(0)] public required long AccountUid { get; set; }
    [Key(1)] public required string Username { get; set; }
    [Key(2)] public string? Email { get; set; }
    [Key(3)] public required string DisplayName { get; set; }
    [Key(4)] public required string Password { get; set; }
    [Key(5)] public required string RoleName { get; set; }
}
