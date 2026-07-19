namespace Product.Common.Catalog.GatewayConsul;

/// <summary>
/// 连接独立运行时侧 Consul，从 KV 读取 provider 目录等。
/// </summary>
public sealed class GatewayConsulOptions
{
    public string Address { get; set; } = "";
    public string? Token { get; set; }
    public string? Datacenter { get; set; }
    public string KvPrefix { get; set; } = "demux";
}
