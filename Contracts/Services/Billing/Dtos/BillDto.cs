using MessagePack;

namespace Meeko.Contracts.Billing;

[MessagePackObject]
public sealed class BillDto
{
    /// <summary>对外形如 "BL20260531000001234"（前缀 + UTC 日期 + 9 位序列）。</summary>
    [Key(0)] public required string Id { get; set; }

    [Key(1)] public required BillOwnerInfo Owner { get; set; }

    [Key(2)] public required BillOperatorInfo Operator { get; set; }

    /// <summary>业务归属与溯源（产品 / 计费类型 / 关联实体 / 请求幂等键 / 调用日志）。</summary>
    [Key(3)] public BillBusinessInfo? Business { get; set; }

    /// <summary>pending / completed / failed / reversed / partial_refunded。</summary>
    [Key(5)] public required string Status { get; set; }

    [Key(6)] public required BillAmountInfo Amount { get; set; }

    [Key(8)] public BillFailureInfo? Failure { get; set; }

    [Key(9)] public BillReversalInfo? Reversal { get; set; }

    [Key(10)] public DateTime OccurredAtUtc { get; set; }

    /// <summary>扣费明细（代金券抵扣 / 余额扣除拆分）；仅用量扣费类账单有值，充值等为 null。</summary>
    [Key(11)] public BillDeductionDto? Deduction { get; set; }
}
