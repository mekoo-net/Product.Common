using MessagePack;

namespace Meeko.Contracts.Billing;

/// <summary>
/// 账单的业务归属与溯源块：产品 / 计费类型 / 关联业务实体 / 发起调用日志。
/// 与前端 BillingEntry.business 一一对应。
/// </summary>
[MessagePackObject]
public sealed class BillBusinessInfo
{
    /// <summary>产品代码；扣款/充值归属产品时有值。</summary>
    [Key(1)] public string? ProductCode { get; set; }

    /// <summary>计费类型：prepaid / usage；无法判定时为 null。</summary>
    [Key(2)] public string? SubType { get; set; }

    /// <summary>关联业务实体类型：recharge / hold / manual。</summary>
    [Key(3)] public string? RefType { get; set; }

    /// <summary>关联业务实体号（如 hold 单号 HD-xxx、充值流水号）。</summary>
    [Key(4)] public string? RefId { get; set; }

    /// <summary>
    /// 产品域请求幂等键（= Commit 流水 IdempotencyKey）。产品域据此把账单流水反查回
    /// 发起它的业务日志（如 Demux 调用日志）；非 Commit 类账单为 null。
    /// </summary>
    [Key(5)] public string? RequestId { get; set; }

    /// <summary>
    /// 发起本次扣费的业务日志号（产品域 UsageLog.Id）。Billing 域不感知，恒为 null，
    /// 由产品域（console / 产品 BFF）按 RequestId 跨域解析后回填。
    /// </summary>
    [Key(6)] public string? OriginLogId { get; set; }
}
