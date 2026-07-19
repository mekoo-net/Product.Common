using MessagePack;

namespace Meeko.Contracts.Billing;

/// <summary>产品维度的已落账消费聚合结果。</summary>
[MessagePackObject]
public sealed class ProductUsageSummary
{
    /// <summary>窗口内 Commit 流水合计金额（单位与钱包一致，CNY）。</summary>
    [Key(0)] public decimal Amount { get; set; }
}
