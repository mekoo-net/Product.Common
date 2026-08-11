using MessagePack;

namespace Meeko.Contracts.Insight;

/// <summary>
/// 一条 trace 对应的业务事实。
///
/// <para><b>除了头部几个字段，明细一律走 <see cref="Sections"/> 的 分组 / 行 形态，
/// 控制台只渲染、不理解语义。</b> 强类型化的代价是每个产品加一个计费维度（新的 token 类型、
/// 新的按次计价档位）控制台就得跟着改一次并重新部署——而控制台对这些字段唯一要做的事
/// 就是把它显示出来。代价是这些字段不能用来筛选或聚合，那部分能力留给 span 属性。</para>
///
/// <para><b>⚠️ Key(n) 是数组下标，只能追加。</b> 本类型天生就要长字段，这条比别处更要命：
/// 插到中间或重排会让未升级的一端静默错位（把 Status 当 Kind 读），不报错、最难查。
/// 见根目录 <c>CLAUDE.md</c> §3。下一个空位：<b>9</b>。</para>
/// </summary>
[MessagePackObject]
public sealed class TraceInsight
{
    /// <summary>请求里那个 traceId 的回声，调用方据此把批量结果对回请求。</summary>
    [Key(0)] public string Ref { get; set; } = string.Empty;

    /// <summary>产品自定的业务类型，如 <c>llm.call</c> / <c>tavern.turn</c>。控制台只当标签显示。</summary>
    [Key(1)] public string Kind { get; set; } = string.Empty;

    /// <summary>
    /// 是否已是终态。false = 还在结算（reserve 了但没 commit），金额随时会变，
    /// <b>调用方不得缓存</b>，也不该拿去对账。
    /// </summary>
    [Key(2)] public bool Settled { get; set; }

    [Key(3)] public string Status { get; set; } = string.Empty;
    [Key(4)] public decimal? Amount { get; set; }
    [Key(5)] public string? Currency { get; set; }
    [Key(6)] public DateTime? OccurredAtUtc { get; set; }
    [Key(7)] public InsightSection[] Sections { get; set; } = [];

    /// <summary>账单号 / 日志号一类的外部引用，供人工去产品管理台继续查。</summary>
    [Key(8)] public InsightLink[] Links { get; set; } = [];
}

/// <summary>下一个空位：<b>2</b>。</summary>
[MessagePackObject]
public sealed class InsightSection
{
    [Key(0)] public string Title { get; set; } = string.Empty;
    [Key(1)] public InsightRow[] Rows { get; set; } = [];
}

/// <summary>下一个空位：<b>3</b>。</summary>
[MessagePackObject]
public sealed class InsightRow
{
    [Key(0)] public string Label { get; set; } = string.Empty;
    [Key(1)] public string Value { get; set; } = string.Empty;

    /// <summary>单位（<c>token</c> / <c>ms</c> / <c>CNY/M token</c>…）；没有就留空。</summary>
    [Key(2)] public string? Unit { get; set; }
}

/// <summary>下一个空位：<b>2</b>。</summary>
[MessagePackObject]
public sealed class InsightLink
{
    [Key(0)] public string Label { get; set; } = string.Empty;
    [Key(1)] public string Value { get; set; } = string.Empty;
}
