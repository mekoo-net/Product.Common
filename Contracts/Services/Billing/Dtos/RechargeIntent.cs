using System.Text.Json.Serialization;
using Platform.Common.Web;
using MessagePack;

namespace Meeko.Contracts.Billing;

[MessagePackObject]
public sealed class RechargeIntent
{
    [Key(0)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long RechargeId { get; set; }

    [Key(1)] public string OutTradeNo { get; set; } = string.Empty;
    [Key(2)] public string Provider { get; set; } = string.Empty;
    [Key(3)] public PaymentScene Scene { get; set; }
    [Key(4)] public decimal Amount { get; set; }
    [Key(5)] public string Currency { get; set; } = "CNY";
    [Key(6)] public string? QrCodeUrl { get; set; }
    [Key(7)] public string? RedirectUrl { get; set; }
    [Key(8)] public string? JsApiPayloadJson { get; set; }
    [Key(9)] public DateTime CreatedAtUtc { get; set; }
    [Key(10)] public DateTime? ExpiresAtUtc { get; set; }
}
