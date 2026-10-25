using System.Text.Json.Serialization;
using MessagePack;
using Platform.Common.Web;

namespace Meeko.Contracts.Billing;

[MessagePackObject]
public sealed class IssueProductVoucherCommand
{
    [Key(0)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long AccountUid { get; set; }

    [Key(1)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long TemplateId { get; set; }

    /// <summary>发券的产品代码（如 yomi）。模板的适用范围必须包含它；幂等键与来源标记都按它分隔。</summary>
    [Key(2)] public string ProductCode { get; set; } = string.Empty;

    /// <summary>产品内唯一，最长 64。Billing 存为 <c>product:{ProductCode}:{IdempotencyKey}</c>，不同产品互不冲突。</summary>
    [Key(3)] public string IdempotencyKey { get; set; } = string.Empty;

    /// <summary>
    /// 来源归因（如 <c>activity:12</c>），可空。Billing 存为 <c>{ProductCode}:{Origin}</c>，
    /// 拼接后不能超过 64 个字符；按它可以在 Billing 查某个来源发给了谁。
    /// </summary>
    [Key(4)] public string? Origin { get; set; }
}

[MessagePackObject]
public sealed class IssueProductVoucherResult
{
    [Key(0)] public bool Success { get; set; }

    [Key(1)]
    [JsonConverter(typeof(NullableLongToStringConverter))]
    public long? UserVoucherId { get; set; }

    [Key(2)] public string? SerialNo { get; set; }

    /// <summary>validation / not_found / conflict。</summary>
    [Key(3)] public string? FailureCode { get; set; }

    [Key(4)] public string? FailureMessage { get; set; }

    public static IssueProductVoucherResult Issued(long userVoucherId, string? serialNo)
        => new() { Success = true, UserVoucherId = userVoucherId, SerialNo = serialNo };

    public static IssueProductVoucherResult Fail(string code, string message)
        => new() { Success = false, FailureCode = code, FailureMessage = message };
}

/// <summary>产品管理台挑模板、展示已选模板用的精简视图。</summary>
[MessagePackObject]
public sealed class IssuableVoucherTemplateDto
{
    [Key(0)]
    [JsonConverter(typeof(LongToStringConverter))]
    public long Id { get; set; }

    [Key(1)] public string Name { get; set; } = string.Empty;

    /// <summary>no_threshold / full_reduction / discount。</summary>
    [Key(2)] public string DeductKind { get; set; } = string.Empty;

    [Key(3)] public decimal FaceValue { get; set; }

    [Key(4)] public decimal ThresholdAmount { get; set; }

    /// <summary>折扣券的折扣率（0.8 = 八折）。</summary>
    [Key(5)] public decimal? DiscountRate { get; set; }

    /// <summary>absolute（固定起止）/ relative_days（领取后 N 天）。</summary>
    [Key(6)] public string ValidityKind { get; set; } = string.Empty;

    [Key(7)] public DateTime? ValidFromUtc { get; set; }

    [Key(8)] public DateTime? ValidToUtc { get; set; }

    [Key(9)] public int? ValidDays { get; set; }

    [Key(10)] public int? TotalQuota { get; set; }

    [Key(11)] public int IssuedCount { get; set; }

    /// <summary>模板自己的每人限领（与产品侧的限领各管各的，两者都要满足）。</summary>
    [Key(12)] public int? PerUserLimit { get; set; }

    /// <summary>draft / active / paused / archived。</summary>
    [Key(13)] public string Status { get; set; } = string.Empty;

    [Key(14)] public bool AllProducts { get; set; }

    [Key(15)] public string[] ScopeProductCodes { get; set; } = [];
}
