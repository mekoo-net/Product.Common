using MessagePack;

namespace Meeko.Contracts.Billing;

/// <summary>
/// 充值支付凭证；详情接口填充，列表为 null。
/// 自动充值取自三方回调，手工入账取自管理员补录的交易信息。
/// </summary>
[MessagePackObject]
public sealed class RechargePaymentInfo
{
    /// <summary>商户单号（OutTradeNo），平台侧寻址用。</summary>
    [Key(0)] public required string OutTradeNo { get; set; }

    /// <summary>三方交易号 / 银行流水号；待支付或未回调时为 null。</summary>
    [Key(1)] public string? ProviderTradeNo { get; set; }

    /// <summary>实付金额；以三方回调或手工补录为准。</summary>
    [Key(2)] public decimal? PaidAmount { get; set; }

    /// <summary>付款账号（支付宝账号 / 微信 openid / 银行卡尾号等）。</summary>
    [Key(3)] public string? PayerAccount { get; set; }

    /// <summary>付款人姓名。</summary>
    [Key(4)] public string? PayerName { get; set; }

    /// <summary>入账确认方式：auto_notify（渠道回调）/ admin_manual（管理员手工确认）/ internal（内部入账）。</summary>
    [Key(5)] public string? ConfirmationMode { get; set; }
}
