using System.Text.Json.Serialization;
using Platform.Common.Web;
using MessagePack;

namespace Meeko.Contracts.Billing;

[MessagePackObject]
public sealed class CreateInternalRechargeCommand
{
    [Key(0)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long AccountUid { get; set; }

    [Key(1)] public decimal Amount { get; set; }

    /// <summary>manual / cs_compensation / marketing_reward。</summary>
    [Key(2)] public string Source { get; set; } = "manual";

    [Key(3)] public string? Note { get; set; }

    [Key(4)]
    [JsonConverter(typeof(NullableLongToStringConverter))]
    public long? OperatorUid { get; set; }

    [Key(5)] public string? IdempotencyKey { get; set; }

    /// <summary>充值归属产品代码（如 demux）。</summary>
    [Key(6)] public string? ProductCode { get; set; }
}
