using MagicOnion;

namespace Meeko.Contracts.Billing;

/// <summary>产品域 → Billing：hold / commit / release / usage。</summary>
public interface IBillingMeteringService : IService<IBillingMeteringService>
{
    UnaryResult<HoldResult> TryHoldAsync(HoldRequest request);

    /// <summary>
    /// 提交预扣并落账。返回的 <see cref="CommitHoldResult.BillSerialNo"/> 为本次 Commit 流水号（账单号），
    /// 供产品域把业务日志单向引用到账单（写入 UsageLog.BillSerialNo）。
    /// </summary>
    UnaryResult<CommitHoldResult> CommitHoldAsync(long holdId, decimal actualAmount, string idempotencyKey);

    UnaryResult<bool> ReleaseHoldAsync(long holdId, string reason);

    UnaryResult<bool> ReportUsageAsync(ReportUsageRequest request);

    /// <summary>
    /// 按 Commit 幂等键（产品域 request_id）批量回查已落账的钱包流水。
    /// 供产品域把自身用量日志与 Billing 账单关联，避免跨库直读 Billing schema。
    /// </summary>
    UnaryResult<LookupCommitBillsByIdempotencyKeysResult> LookupCommitBillsByIdempotencyKeysAsync(
        string[] idempotencyKeys);

    /// <summary>按 Commit 幂等键全额驳回（退回钱包 + 写 Refund 流水）。</summary>
    UnaryResult<ReverseBillResult> ReverseCommitByIdempotencyKeyAsync(ReverseCommitByKeyCommand cmd);

    /// <summary>
    /// 按 ProductCode 聚合某账户在时间窗内的已落账消费金额（Commit 流水合计），
    /// 供产品域仪表板展示「本产品消费」，避免产品域跨库直读 Billing schema。
    /// </summary>
    UnaryResult<ProductUsageSummary> GetProductUsageSummaryAsync(ProductUsageSummaryQuery query);
}
