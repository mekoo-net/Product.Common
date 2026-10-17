using MessagePack;

namespace Meeko.Contracts.Keystone;

[MessagePackObject]
public sealed class SetAccountStatusCommand
{
    [Key(0)] public required long AccountUid { get; set; }

    /// <summary>"active" / "suspended"；不允许通过 API 设为 deleted。</summary>
    [Key(1)] public required string Status { get; set; }
}
