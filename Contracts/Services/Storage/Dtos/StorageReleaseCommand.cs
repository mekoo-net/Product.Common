using MessagePack;

namespace Meeko.Contracts.Storage.Dtos;

/// <summary>
/// 放开某账号在某产品/用途下的引用。物理对象不动，活跃引用归零后才可以回收。
/// </summary>
[MessagePackObject]
public sealed class StorageReleaseCommand
{
    [Key(0)] public long AccountUid { get; set; }
    [Key(1)] public string Product { get; set; } = string.Empty;
    [Key(2)] public string Purpose { get; set; } = string.Empty;

    /// <summary>锚点前缀。以这个前缀开头的活跃引用整批放开，用来对上一次会话。</summary>
    [Key(3)] public string RefKeyPrefix { get; set; } = string.Empty;

    /// <summary>消息里实际出现过的对象，以及各出现了几次。前缀没盖住的旧引用按这个次数补放。</summary>
    [Key(4)] public StorageReleaseItem[] Items { get; set; } = [];
}

[MessagePackObject]
public sealed class StorageReleaseItem
{
    [Key(0)] public string StorageKey { get; set; } = string.Empty;
    [Key(1)] public int Count { get; set; }
}
