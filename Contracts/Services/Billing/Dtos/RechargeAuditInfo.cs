using MessagePack;

namespace Meeko.Contracts.Billing;

/// <summary>
/// 充值审计信息；详情接口填充，列表为 null。
/// 记录入账操作人、确认时间、备注与失败 / 过期等生命周期信息。
/// </summary>
[MessagePackObject]
public sealed class RechargeAuditInfo
{
    /// <summary>备注（手工入账说明 / 审批单号等）。</summary>
    [Key(0)] public string? Remark { get; set; }

    /// <summary>待支付单过期时间。</summary>
    [Key(1)] public DateTime? ExpiresAtUtc { get; set; }

    /// <summary>失败 / 过期原因。</summary>
    [Key(2)] public string? FailureReason { get; set; }

    /// <summary>手工确认入账的管理员 StaffUser.Uid；非手工确认时为 null。</summary>
    [Key(3)] public long? ConfirmedByStaffUid { get; set; }

    /// <summary>手工确认入账的时间；非手工确认时为 null。</summary>
    [Key(4)] public DateTime? ConfirmedAtUtc { get; set; }
}
