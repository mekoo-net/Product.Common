using MessagePack;

namespace Meeko.Contracts.Billing;

[MessagePackObject]
public sealed class BillAmountInfo
{
    [Key(0)] public required decimal Original { get; set; }
    [Key(1)] public required decimal Actual { get; set; }
    [Key(2)] public required string Currency { get; set; }

    /// <summary>仅 status ∈ {completed, partial_refunded} 时非 null；当前域无历史余额回溯，恒 null。</summary>
    [Key(3)] public decimal? BalanceAfter { get; set; }
}
