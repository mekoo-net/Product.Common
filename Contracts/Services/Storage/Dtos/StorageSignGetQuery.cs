using MessagePack;

namespace Meeko.Contracts.Storage.Dtos;

[MessagePackObject]
public sealed class StorageSignGetQuery
{
    [Key(0)] public string? FinalUrl { get; set; }
    [Key(1)] public string? StorageKey { get; set; }
    [Key(2)] public int TtlSeconds { get; set; } = 600;
    [Key(3)] public long AccountUid { get; set; }
}
