using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Platform.Common.Discovery;
using Platform.Common.Observability;
using Product.Common.Discovery.Manifests;

namespace Product.Common.Discovery;

/// <summary>
/// 把当前产品服务（Yomi/Tavern/ToApi/...）注册到 Consul 并维持心跳；
/// 进程退出时尽力 deregister。Service Meta 同时携带 <see cref="ServiceRouteManifest"/> 编码的前缀路由表，
/// 网关 watcher 通过 catalog 拉取后反序列化重建 YARP 路由。
/// </summary>
internal sealed class ConsulServiceRegistrar : IHostedService
{
    private readonly IConsulClient _consul;
    private readonly LocalServiceInstance _self;
    private readonly GatewayManifestOptions _manifest;
    private readonly IReadOnlyList<ProductDeclaration> _products;
    private readonly IReadOnlyList<ScheduledTaskDeclaration> _schedules;
    private readonly IConfiguration _configuration;
    private readonly ObservabilityOptions? _observability;
    private readonly ILogger<ConsulServiceRegistrar> _logger;
    private string? _registeredId;

    /// <param name="observability">
    /// 走 <c>AddPlatformObservability</c> 的宿主才有；Yomi.Gateway 自己装配 OTel，没有这个单例，
    /// 世界声明退回从 <paramref name="configuration"/> 读。
    /// </param>
    public ConsulServiceRegistrar(
        IConsulClient consul,
        LocalServiceInstance self,
        IOptions<ProductDiscoveryOptions> productDiscovery,
        IConfiguration configuration,
        ILogger<ConsulServiceRegistrar> logger,
        ObservabilityOptions? observability = null)
    {
        _consul = consul;
        _self = self;
        _manifest = productDiscovery.Value.Gateway;
        _products = productDiscovery.Value.Products;
        _schedules = productDiscovery.Value.Schedules;
        _configuration = configuration;
        _observability = observability;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var routesJson = ServiceRouteManifest.Encode(_manifest.Routes);
        var check = _self.CreateHealthCheck();

        var meta = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [ServiceRouteManifest.MetaKey] = routesJson,
            [ServiceRouteManifest.BaseUrlMetaKey] = _self.RestBaseUrl.ToString(),
        };
        if (_self.GrpcBaseUrl is { } grpcUrl)
        {
            meta[ServiceRouteManifest.GrpcUrlMetaKey] = grpcUrl.GetLeftPart(UriPartial.Authority);
        }
        if (_products.Count > 0)
        {
            meta[ProductCatalogManifest.MetaKey] = ProductCatalogManifest.Encode(_products);
        }
        if (_schedules.Count > 0)
        {
            meta[ScheduleManifest.MetaKey] = ScheduleManifest.Encode(_schedules);
        }
        var world = WorldServiceMeta.Write(meta, _configuration, _observability);

        var registration = new AgentServiceRegistration
        {
            ID = _self.Id,
            Name = _self.Key,
            Address = _self.Host,
            Port = _self.RestBaseUrl.Port,
            Tags = ["meeko"],
            Meta = meta,
            Check = check,
        };

        try
        {
            await _consul.Agent.ServiceRegister(registration, cancellationToken);
            _registeredId = _self.Id;
            var prefixes = string.Join(", ", _manifest.Routes.Select(r => r.Prefix));
            _logger.LogInformation(
                "Registered service '{Service}' (id={Id}) at {Address} with {RouteCount} route prefix(es) [{Prefixes}]; health={Health}; grpc={Grpc}; world={World}",
                _self.Key, _self.Id, _self.RestBaseUrl, _manifest.Routes.Count, prefixes, check.HTTP, _self.GrpcBaseUrl,
                world is null ? "(none)" : $"{world.Key} as {world.ServiceName}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Best-effort Consul register failed for '{Service}' at {Address}; service will still start",
                _self.Key, _self.RestBaseUrl);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_registeredId))
        {
            return;
        }

        try
        {
            await _consul.Agent.ServiceDeregister(_registeredId, cancellationToken);
            _logger.LogInformation("Deregistered service '{Id}' from Consul", _registeredId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Best-effort Consul deregister failed for '{Id}'", _registeredId);
        }
    }
}
