using MessagePack;

namespace Meeko.Contracts.Keystone;

[MessagePackObject]
public sealed class AccountContactBatchResult
{
    [Key(0)] public required AccountContactDto[] Items { get; set; }
}
