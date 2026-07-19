using System.Text.Json.Serialization;
using Platform.Common.Web;
using MessagePack;

namespace Meeko.Contracts.Billing;

[MessagePackObject]
public sealed class WalletSnapshot
{
    [Key(0)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long AccountUid { get; set; }

    [Key(1)] public decimal Available { get; set; }
    [Key(2)] public decimal Held { get; set; }
    [Key(3)] public string Currency { get; set; } = "CNY";
    [Key(4)] public DateTime UpdatedAtUtc { get; set; }
}
