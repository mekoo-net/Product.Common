using MessagePack;

namespace Meeko.Contracts.Storage.Dtos;

[MessagePackObject]
public sealed class StorageSignPutCommand
{
    [Key(0)] public long AccountUid { get; set; }
    [Key(1)] public string Product { get; set; } = string.Empty;
    [Key(2)] public string Purpose { get; set; } = string.Empty;
    [Key(3)] public string Mime { get; set; } = string.Empty;
    [Key(4)] public long Size { get; set; }
    [Key(5)] public string? Sha256 { get; set; }

    /// <summary>可选业务锚点（personaId / messageId 等）。同一账号从多个业务位置引用同一文件时用于区分来源。</summary>
    [Key(6)] public string? RefKey { get; set; }
}
