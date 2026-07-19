using MessagePack;

namespace Meeko.Contracts.Keystone;

[MessagePackObject]
public sealed class AccountOwnerInfo
{
    [Key(0)] public required string DisplayName { get; set; }
    [Key(1)] public string? Email { get; set; }
    [Key(2)] public string? Phone { get; set; }
}
