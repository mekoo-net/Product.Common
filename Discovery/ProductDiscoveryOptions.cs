namespace Product.Common.Discovery;

/// <summary>
/// Meeko 平台 Consul manifest 配置（YAML 节 <c>Discovery:*</c>）。
/// 这里只放随代码走的声明；Consul 地址在 <c>ConnectionStrings:Consul</c>，本服务的 Key 写死在宿主代码里。
/// </summary>
public sealed class ProductDiscoveryOptions
{
    public const string SectionName = "Discovery";

    public GatewayManifestOptions Gateway { get; set; } = new();
    public List<ProductDeclaration> Products { get; set; } = [];
    public List<ScheduledTaskDeclaration> Schedules { get; set; } = [];
}
