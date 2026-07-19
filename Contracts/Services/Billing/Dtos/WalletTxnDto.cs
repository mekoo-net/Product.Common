using System.Text.Json.Serialization;
using Platform.Common.Web;
using MessagePack;

namespace Meeko.Contracts.Billing;

[MessagePackObject]
public sealed class WalletTxnDto
{
    [Key(0)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long Id { get; set; }

    [Key(1)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long AccountUid { get; set; }

    [Key(2)] public WalletTxnKind Kind { get; set; }
    [Key(3)] public decimal Amount { get; set; }
    [Key(4)] public decimal DeltaAvailable { get; set; }
    [Key(5)] public decimal DeltaHeld { get; set; }
    [Key(6)] public WalletTxnReferenceKind ReferenceKind { get; set; }

    [Key(7)]
    [JsonConverter(typeof(NullableLongToStringConverter))]
    public long? ReferenceId { get; set; }

    [Key(8)] public string? IdempotencyKey { get; set; }
    [Key(9)] public DateTime OccurredAtUtc { get; set; }
}
