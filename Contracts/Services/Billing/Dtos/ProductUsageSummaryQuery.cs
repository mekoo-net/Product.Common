using System.Text.Json.Serialization;
using Platform.Common.Web;
using MessagePack;

namespace Meeko.Contracts.Billing;

/// <summary>按产品聚合某账户在 [FromUtc, ToUtc) 内已落账消费金额的查询。</summary>
[MessagePackObject]
public sealed class ProductUsageSummaryQuery
{
    [Key(0)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long AccountUid { get; set; }

    [Key(1)] public string ProductCode { get; set; } = string.Empty;

    /// <summary>统计窗口起（含），UTC。</summary>
    [Key(2)] public DateTime FromUtc { get; set; }

    /// <summary>统计窗口止（不含），UTC。</summary>
    [Key(3)] public DateTime ToUtc { get; set; }
}
