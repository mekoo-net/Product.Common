using System.Text.Json.Serialization;
using Platform.Common.Web;
using MessagePack;

namespace Meeko.Contracts.Billing;

[MessagePackObject]
public sealed class HoldRequest
{
    [Key(0)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long AccountUid { get; set; }

    [Key(1)] public string ProductCode { get; set; } = string.Empty;
    [Key(2)] public decimal EstimateAmount { get; set; }
    [Key(4)] public WalletTxnReferenceKind ReferenceKind { get; set; } = WalletTxnReferenceKind.None;

    [Key(5)]
    [JsonConverter(typeof(NullableLongToStringConverter))]
    public long? ReferenceId { get; set; }

    [Key(6)] public TimeSpan? Ttl { get; set; }
    [Key(7)] public string IdempotencyKey { get; set; } = string.Empty;
}
