using MessagePack;

namespace Meeko.Contracts.Billing;

/// <summary>
/// Commit 提交结果。<see cref="BillSerialNo"/> 是本次落账的 Commit 流水号（账单号），
/// 产品域据此把自身业务日志单向引用到账单流水（写入 UsageLog.BillSerialNo）。
/// 纯券抵扣（无钱包实付，未生成 Commit 流水）或幂等回放查不到时为 null。
/// </summary>
[MessagePackObject]
public sealed class CommitHoldResult
{
    [Key(0)] public bool Success { get; set; }

    /// <summary>落账的 Commit 流水号（账单号，如 BL20260531000001234）；无钱包实付时为 null。</summary>
    [Key(1)] public string? BillSerialNo { get; set; }

    public static CommitHoldResult Ok(string? billSerialNo) =>
        new() { Success = true, BillSerialNo = billSerialNo };

    public static CommitHoldResult Fail() => new() { Success = false };
}
