using System.Text.Json.Serialization;
using Platform.Common.Web;
using MessagePack;

namespace Meeko.Contracts.Billing;

/// <summary>
/// 账单扣费明细聚合对象。一笔用量扣费账单的"钱从哪来"拆分：先用代金券抵扣，
/// 不足部分再扣钱包余额。仅用量扣费（Commit）类账单有值，充值等加钱类为 null。
/// </summary>
[MessagePackObject]
public sealed class BillDeductionDto
{
    /// <summary>应扣总额（= 代金券抵扣 + 余额扣除），等于账单原价。</summary>
    [Key(0)] public decimal Total { get; set; }

    /// <summary>代金券抵扣合计。</summary>
    [Key(1)] public decimal VoucherDeducted { get; set; }

    /// <summary>钱包余额实际扣除额。</summary>
    [Key(2)] public decimal BalanceDeducted { get; set; }

    /// <summary>各张代金券的抵扣明细（按用户券聚合）。</summary>
    [Key(3)] public BillVoucherDeductionDto[] VoucherItems { get; set; } = [];
}

/// <summary>单张代金券在某账单上的抵扣额。</summary>
[MessagePackObject]
public sealed class BillVoucherDeductionDto
{
    [Key(0)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long UserVoucherId { get; set; }

    /// <summary>券面序列号（如 VC...），便于对账定位；可空。</summary>
    [Key(2)] public string? SerialNo { get; set; }

    [Key(1)] public decimal AmountDeducted { get; set; }

    /// <summary>券名称（取自券模板，发放后快照展示用），便于一眼看出用的是哪张券。</summary>
    [Key(3)] public string? Name { get; set; }

    /// <summary>券抵扣类型：noThreshold（无门槛代金券）/ fullReduction（满减券）/ discount（折扣券）。</summary>
    [Key(4)] public string? DeductKind { get; set; }

    /// <summary>券面额（发放时快照的初始可抵扣金额）。</summary>
    [Key(5)] public decimal FaceValue { get; set; }

    /// <summary>该券当前剩余可抵扣额（查询时刻快照，非本笔账单抵扣后的历史值）。</summary>
    [Key(6)] public decimal RemainingValue { get; set; }

    /// <summary>券有效期截止时间（UTC）。</summary>
    [Key(7)] public DateTime ValidToUtc { get; set; }

    /// <summary>使用门槛金额（满减/折扣券有意义，无门槛券为 0）。</summary>
    [Key(8)] public decimal ThresholdAmount { get; set; }

    /// <summary>折扣率（仅折扣券有值，区间 (0,1)）。</summary>
    [Key(9)] public decimal? DiscountRate { get; set; }
}
