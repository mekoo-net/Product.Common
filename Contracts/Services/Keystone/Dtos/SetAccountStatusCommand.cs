using MessagePack;

namespace Meeko.Contracts.Keystone;

[MessagePackObject]
public sealed class SetAccountStatusCommand
{
    [Key(0)] public required long AccountUid { get; set; }

    /// <summary>"active" / "suspended"；不允许通过 API 设为 deleted。</summary>
    [Key(1)] public required string Status { get; set; }
}

[MessagePackObject]
public sealed class SetAccountTierCommand
{
    [Key(0)] public required long AccountUid { get; set; }

    /// <summary>目标等级（1..5），由管理员手动授予。</summary>
    [Key(1)] public required int Tier { get; set; }
}
