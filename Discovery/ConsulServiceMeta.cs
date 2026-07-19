namespace Product.Common.Discovery;

/// <summary>Consul service meta key names (Meeko platform wire protocol).</summary>
public static class ConsulServiceMeta
{
    public const string Routes = "meeko-routes";
    public const string BaseUrl = "meeko-base-url";
    public const string PublicUrl = "meeko-public-url";
    public const string GrpcUrl = "meeko-grpc-url";
    public const string Products = "meeko-products";
    public const string Schedules = "meeko-schedules";
}
