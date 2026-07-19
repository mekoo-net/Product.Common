using System.Text.Json.Serialization;
using Platform.Common.Web;
using MessagePack;

namespace Meeko.Contracts.Billing;

[MessagePackObject]
public sealed class ReportUsageRequest
{
    [Key(0)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long AccountUid { get; set; }

    [Key(1)] public string ProductCode { get; set; } = string.Empty;
    [Key(3)] public string MetricKey { get; set; } = string.Empty;
    [Key(4)] public decimal Quantity { get; set; }
    [Key(5)] public decimal UnitPrice { get; set; }
    [Key(6)] public decimal Amount { get; set; }

    [Key(7)]
    [JsonConverter(typeof(NullableLongToStringConverter))]
    public long? HoldId { get; set; }

    [Key(8)] public DateTime OccurredAtUtc { get; set; }
    [Key(9)] public string IdempotencyKey { get; set; } = string.Empty;
}
