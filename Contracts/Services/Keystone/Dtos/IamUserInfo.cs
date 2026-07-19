using MessagePack;

namespace Meeko.Contracts.Keystone;

[MessagePackObject]
public sealed class IamUserInfo
{
    [Key(0)] public required long Uid { get; set; }
    [Key(1)] public required long AccountUid { get; set; }
    [Key(2)] public required string Username { get; set; }
    [Key(3)] public string? Email { get; set; }
    [Key(4)] public required string DisplayName { get; set; }
    [Key(5)] public required string Role { get; set; }
    [Key(6)] public required bool IsAccountOwner { get; set; }

    /// <summary>active / disabled / locked。</summary>
    [Key(7)] public required string Status { get; set; }
}
