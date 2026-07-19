using System.Text.Json.Serialization;
using Platform.Common.Web;
using MessagePack;

namespace Meeko.Contracts.Billing;

[MessagePackObject]
public sealed class CreateRechargeCommand
{
    [Key(0)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long AccountUid { get; set; }

    [Key(1)] public decimal Amount { get; set; }
    [Key(2)] public string Provider { get; set; } = "manual";
    [Key(3)] public PaymentScene Scene { get; set; } = PaymentScene.Manual;

    /// <summary>
    /// 选定的支付渠道实例 Id（PaymentChannel.Id）。多实例下用于精确定位配置；
    /// 为 0 时退化为按 <see cref="Provider"/> 选取首个启用实例（手工/内部入账场景）。
    /// </summary>
    [Key(4)] public long ChannelId { get; set; }

    [Key(5)] public string? ClientIp { get; set; }
    [Key(6)] public string? ReturnUrl { get; set; }
    [Key(7)] public string? OpenId { get; set; }
    [Key(8)] public string? Subject { get; set; } = "Meeko 钱包充值";
    [Key(9)] public string? IdempotencyKey { get; set; }

    /// <summary>充值归属产品代码（如 demux）。</summary>
    [Key(10)] public string? ProductCode { get; set; }

    /// <summary>
    /// 渠道专属下单参数（JSON）。通用充值流程不感知其结构，由对应渠道自行解析。
    /// 发卡付示例：<c>{"backendId":1,"goodsKey":"uacyrv","channelId":1}</c>
    /// </summary>
    [Key(11)] public string? Payload { get; set; }
}
