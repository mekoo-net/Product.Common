using MessagePack;

namespace Meeko.Contracts.Billing;

/// <summary>
/// 发卡付下单前的收款选项：每个后端（发卡网）下的商品（固定金额档位）与支付渠道（支付宝/微信等）。
/// 由 BFF / 前端在用户选择「发卡付」后拉取并展示，用户先选商品（决定金额）再选支付渠道。
/// </summary>
[MessagePackObject]
public sealed class FkPayOptionsDto
{
    [Key(0)] public FkPayBackendDto[] Backends { get; set; } = [];
}

[MessagePackObject]
public sealed class FkPayBackendDto
{
    [Key(0)] public long BackendId { get; set; }
    [Key(1)] public string BackendName { get; set; } = string.Empty;
    [Key(2)] public FkPayGoodsDto[] Goods { get; set; } = [];
    [Key(3)] public FkPayChannelDto[] Channels { get; set; } = [];
}

[MessagePackObject]
public sealed class FkPayGoodsDto
{
    /// <summary>商品金额（元，固定档位）。</summary>
    [Key(0)] public decimal Amount { get; set; }

    /// <summary>商品标识，下单时回传。</summary>
    [Key(1)] public string GoodsKey { get; set; } = string.Empty;

    /// <summary>商品名称（展示用）。</summary>
    [Key(2)] public string Name { get; set; } = string.Empty;
}

[MessagePackObject]
public sealed class FkPayChannelDto
{
    [Key(0)] public int ChannelId { get; set; }

    /// <summary>渠道代码，如 AlipayPc / WeixinNative。</summary>
    [Key(1)] public string Code { get; set; } = string.Empty;

    /// <summary>渠道名称（展示用），如 支付宝电脑收款。</summary>
    [Key(2)] public string PayTypeName { get; set; } = string.Empty;

    [Key(3)] public string? PayTypeIcon { get; set; }
}
