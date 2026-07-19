using MessagePack;

namespace Meeko.Contracts.Billing;

/// <summary>
/// 按幂等键批量解析 Commit 钱包流水（Demux usage_logs.request_id → WalletTxn.idempotency_key）。
/// </summary>
[MessagePackObject]
public sealed class LookupCommitBillsByIdempotencyKeysResult
{
    [Key(0)] public required CommitBillLookupItem[] Items { get; set; }
}

[MessagePackObject]
public sealed class CommitBillLookupItem
{
    [Key(0)] public string IdempotencyKey { get; set; } = string.Empty;
    [Key(1)] public string BillId { get; set; } = string.Empty;
    [Key(2)] public string Status { get; set; } = "completed";
    [Key(3)] public DateTime? ReversedAtUtc { get; set; }
    [Key(4)] public string? ReversedCode { get; set; }
}
