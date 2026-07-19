using MessagePack;

namespace Meeko.Contracts.Keystone;

[MessagePackObject]
public sealed class AccountInfo
{
    [Key(0)] public required long Uid { get; set; }

    /// <summary>personal / organization。</summary>
    [Key(1)] public required string Type { get; set; }

    /// <summary>Personal = 昵称；Organization = 组织名。</summary>
    [Key(2)] public required string DisplayName { get; set; }

    /// <summary>active / suspended / deleted。</summary>
    [Key(3)] public required string Status { get; set; }
}
