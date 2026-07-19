using MessagePack;

namespace Meeko.Contracts.Billing;

/// <summary>
/// 管理员手工确认待支付充值单入账（含三方支付掉单补录）。
/// 按 <see cref="SerialNo"/> 定位待支付充值单，确认后置为已支付并给钱包加余额，
/// 记录入账操作人 <see cref="OperatorStaffUid"/> 与补录的支付凭证。
/// </summary>
[MessagePackObject]
public sealed class ConfirmManualRechargeCommand
{
    /// <summary>充值流水号（对外展示号，形如 RC20260531000001234）。</summary>
    [Key(0)] public required string SerialNo { get; set; }

    /// <summary>三方交易号 / 银行流水号（可选）。</summary>
    [Key(1)] public string? ProviderTradeNo { get; set; }

    /// <summary>付款账号（可选）。</summary>
    [Key(2)] public string? PayerAccount { get; set; }

    /// <summary>付款人姓名（可选）。</summary>
    [Key(3)] public string? PayerName { get; set; }

    /// <summary>备注（可选）。</summary>
    [Key(4)] public string? Remark { get; set; }

    /// <summary>执行入账的管理员 StaffUser.Uid。</summary>
    [Key(5)] public long? OperatorStaffUid { get; set; }
}
