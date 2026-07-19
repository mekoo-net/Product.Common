namespace Product.Common.Discovery;

/// <summary>
/// Meeko 平台 Consul manifest 配置（YAML 节仍挂在 <c>Discovery:*</c> 下）。
/// </summary>
public sealed class ProductDiscoveryOptions
{
    public GatewayManifestOptions Gateway { get; set; } = new();
    public List<ProductDeclaration> Products { get; set; } = [];
    public List<ScheduledTaskDeclaration> Schedules { get; set; } = [];
}
