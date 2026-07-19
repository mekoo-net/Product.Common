using MessagePack;

namespace Meeko.Contracts.Storage.Dtos;

/// <summary>
/// Confirm 结果。上传经 staging 中转，最终内容寻址 key 由服务端验完哈希才确定，
/// 调用方必须持久化这里返回的 <see cref="StorageKey"/>（而不是 sign 阶段的 staging key）。
/// </summary>
[MessagePackObject]
public sealed class StorageConfirmResult
{
    [Key(0)] public bool Success { get; set; }

    /// <summary>最终 storage key。业务表持久化这个。</summary>
    [Key(1)] public string StorageKey { get; set; } = string.Empty;

    /// <summary>公开层返回 CDN URL；私有层为 null，需经 SignGet 鉴权读取。</summary>
    [Key(2)] public string? FinalUrl { get; set; }
    [Key(3)] public string? FailureCode { get; set; }
    [Key(4)] public string? FailureMessage { get; set; }

    public static StorageConfirmResult Ok(string storageKey, string? finalUrl)
        => new() { Success = true, StorageKey = storageKey, FinalUrl = finalUrl };

    public static StorageConfirmResult Fail(string code, string msg)
        => new() { FailureCode = code, FailureMessage = msg };
}
