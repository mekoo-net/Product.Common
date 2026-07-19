namespace Product.Common.Discovery;

public sealed class GatewayManifestOptions
{
    public List<RouteManifest> Routes { get; set; } = [];
}

/// <summary>Scheduled task declaration for Consul schedule manifest encoding.</summary>
public sealed class ScheduledTaskDeclaration
{
    public string TaskKey { get; set; } = string.Empty;
    public string Cron { get; set; } = string.Empty;
}

/// <summary>Service meta entry for product/billing registration.</summary>
public sealed class ProductDeclaration
{
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

/// <summary>
/// 单条前缀路由声明：<paramref name="Prefix"/> + 鉴权 policy + 是否剥离前缀。
/// </summary>
public sealed class RouteManifest
{
    public string Prefix { get; set; } = string.Empty;

    /// <summary>
    /// YARP <c>RouteConfig.AuthorizationPolicy</c>：
    /// <c>Anonymous</c> / <c>Default</c> / 已注册的 policy 名（如 <c>StaffOnly</c>）。
    /// </summary>
    public string Policy { get; set; } = "Default";

    public bool StripPrefix { get; set; }
}
