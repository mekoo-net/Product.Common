using System.Text.Json.Serialization;
using Platform.Common.Web;
using MessagePack;

namespace Meeko.Contracts.Billing;

[MessagePackObject]
public sealed class HoldResult
{
    [Key(0)] public bool Success { get; set; }

    [Key(1)]
    [JsonConverter(typeof(NullableLongToStringConverter))]
    public long? HoldId { get; set; }

    [Key(2)] public decimal? AvailableAfter { get; set; }
    [Key(3)] public string? FailureCode { get; set; }
    [Key(4)] public string? FailureMessage { get; set; }
    [Key(5)] public DateTime? ExpiresAtUtc { get; set; }
}
