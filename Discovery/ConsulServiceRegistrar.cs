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
/// 把当前产品服务（Yomi/Tavern/ToApi/...）注册到 Consul，并一直守着这条注册；进程退出时尽力 deregister。
/// Service Meta 同时携带 <see cref="ServiceRouteManifest"/> 编码的前缀路由表，
/// 网关 watcher 通过 catalog 拉取后反序列化重建 YARP 路由。
/// </summary>
/// <remarks>
/// 以前只在启动时注册一次。注册一旦从 Consul 里没了——健康检查连续 1 分钟不通被摘、Consul 重启丢了、
/// 启动那一刻 Consul 恰好连不上——就再也回不来，平台网关上这个服务的路由跟着消失（整片 404），只能重启服务。
/// 现在每 <see cref="CheckInterval"/> 问一次 agent 自己还在不在，不在就重新注册；连不上 Consul 只记警告、下一轮再试。
/// 只在缺失时才注册：重复注册会把健康检查重置成 critical，健康过滤的调用方会短暂看不到这个实例。
/// </remarks>
internal sealed class ConsulServiceRegistrar : BackgroundService
{
    private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(30);

    private readonly IConsulClient _consul;
    private readonly LocalServiceInstance _self;
    private readonly GatewayManifestOptions _manifest;
    private readonly IReadOnlyList<ProductDeclaration> _products;
    private readonly IReadOnlyList<ScheduledTaskDeclaration> _schedules;
    private readonly IConfiguration _configuration;
    private readonly ObservabilityOptions? _observability;
    private readonly ILogger<ConsulServiceRegistrar> _logger;
    private AgentServiceRegistration? _registration;
    private WorldDeclaration? _world;
    private bool _registeredOnce;

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

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        // 注册内容只算一次：声明写错（比如树上的路径不合法）在启动时就暴露，而不是在后台循环里每轮报。
        _registration = BuildRegistration();
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var registration = _registration!;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await EnsureRegisteredAsync(registration, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Consul register for '{Service}' at {Address} failed; retrying in {Interval}s",
                    _self.Key, _self.RestBaseUrl, CheckInterval.TotalSeconds);
            }

            try
            {
                await Task.Delay(CheckInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }

    private async Task EnsureRegisteredAsync(AgentServiceRegistration registration, CancellationToken ct)
    {
        var present = await _consul.Agent.Services(ct);
        if (present.Response is { } services && services.ContainsKey(registration.ID))
            return;

        await _consul.Agent.ServiceRegister(registration, ct);

        if (_registeredOnce)
        {
            _logger.LogWarning(
                "Service '{Service}' (id={Id}) was missing from Consul; registered it again at {Address}",
                _self.Key, registration.ID, _self.RestBaseUrl);
            return;
        }

        _registeredOnce = true;
        var prefixes = string.Join(", ", _manifest.Routes.Select(r => r.Prefix));
        _logger.LogInformation(
            "Registered service '{Service}' (id={Id}) at {Address} with {RouteCount} route prefix(es) [{Prefixes}]; health={Health}; grpc={Grpc}; world={World}",
            _self.Key, registration.ID, _self.RestBaseUrl, _manifest.Routes.Count, prefixes, registration.Check?.HTTP, _self.GrpcBaseUrl,
            _world is null ? "(none)" : $"{_world.Path} as {_world.ServiceName}");
    }

    private AgentServiceRegistration BuildRegistration()
    {
        var meta = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [ServiceRouteManifest.MetaKey] = ServiceRouteManifest.Encode(_manifest.Routes),
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
        _world = WorldServiceMeta.Write(meta, _configuration, _observability);

        return new AgentServiceRegistration
        {
            ID = _self.Id,
            Name = _self.Key,
            Address = _self.Host,
            Port = _self.RestBaseUrl.Port,
            Tags = ["meeko"],
            Meta = meta,
            Check = _self.CreateHealthCheck(),
        };
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        if (!_registeredOnce || _registration is null)
        {
            return;
        }

        try
        {
            await _consul.Agent.ServiceDeregister(_registration.ID, cancellationToken);
            _logger.LogInformation("Deregistered service '{Id}' from Consul", _registration.ID);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Best-effort Consul deregister failed for '{Id}'", _registration.ID);
        }
    }
}
