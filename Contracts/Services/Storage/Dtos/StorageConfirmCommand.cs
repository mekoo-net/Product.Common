using MessagePack;

namespace Meeko.Contracts.Storage.Dtos;

[MessagePackObject]
public sealed class StorageConfirmCommand
{
    [Key(0)] public string StorageKey { get; set; } = string.Empty;
    [Key(1)] public long AccountUid { get; set; }
}
