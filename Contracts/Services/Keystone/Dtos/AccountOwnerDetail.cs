using MessagePack;

namespace Meeko.Contracts.Keystone;

[MessagePackObject]
public sealed class AccountOwnerDetail
{
    [Key(0)] public required long IamUserUid { get; set; }
    [Key(1)] public required string DisplayName { get; set; }
    [Key(2)] public string? Email { get; set; }
    [Key(3)] public string? Phone { get; set; }
}
