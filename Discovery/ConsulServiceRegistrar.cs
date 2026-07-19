using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Platform.Common.Discovery;
using Product.Common.Discovery.Manifests;

namespace Product.Common.Discovery;

internal sealed class ConsulServiceRegistrar : IHostedService
{
    private readonly IConsulClient _consul;
    private readonly ServiceOptions _service;
    private readonly GatewayManifestOptions _manifest;
    private readonly IReadOnlyList<ProductDeclaration> _products;
    private readonly IReadOnlyList<ScheduledTaskDeclaration> _schedules;
    private readonly ILogger<ConsulServiceRegistrar> _logger;
    private string? _registeredId;

    public ConsulServiceRegistrar(
        IConsulClient consul,
        IOptions<DiscoveryOptions> discovery,
        IOptions<ProductDiscoveryOptions> productDiscovery,
        ILogger<ConsulServiceRegistrar> logger)
    {
        _consul = consul;
        _service = discovery.Value.Service
            ?? throw new InvalidOperationException(
                "Discovery:Service section is required to register with Consul.");
        _manifest = productDiscovery.Value.Gateway;
        _products = productDiscovery.Value.Products;
        _schedules = productDiscovery.Value.Schedules;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_service.Name))
        {
            throw new InvalidOperationException("Discovery:Service:Name must not be empty.");
        }

        if (!Uri.TryCreate(_service.Address, UriKind.Absolute, out var baseUri))
        {
            throw new InvalidOperationException(
                $"Discovery:Service:Address ('{_service.Address}') is not a valid absolute URI.");
        }

        var serviceId = $"{_service.Name}@{Environment.MachineName}-{Environment.ProcessId}";
        var routesJson = ServiceRouteManifest.Encode(_manifest.Routes);
        var healthUrl = BuildHealthCheckUrl(baseUri, _service.HealthCheck.Path);
        var grpcUrl = ResolveGrpcUrl(baseUri);

        var meta = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [ServiceRouteManifest.MetaKey] = routesJson,
            [ServiceRouteManifest.BaseUrlMetaKey] = NormalizeBaseUrl(baseUri),
            [ServiceRouteManifest.GrpcUrlMetaKey] = grpcUrl,
        };
        if (_products.Count > 0)
        {
            meta[ProductCatalogManifest.MetaKey] = ProductCatalogManifest.Encode(_products);
        }
        if (_schedules.Count > 0)
        {
            meta[ScheduleManifest.MetaKey] = ScheduleManifest.Encode(_schedules);
        }

        var registration = new AgentServiceRegistration
        {
            ID = serviceId,
            Name = _service.Name,
            Address = baseUri.Host,
            Port = baseUri.Port,
            Tags = ["meeko"],
            Meta = meta,
            Check = new AgentServiceCheck
            {
                HTTP = healthUrl,
                Interval = _service.HealthCheck.Interval,
                Timeout = _service.HealthCheck.Timeout,
                DeregisterCriticalServiceAfter = _service.HealthCheck.DeregisterAfter,
            },
        };

        try
        {
            await _consul.Agent.ServiceRegister(registration, cancellationToken);
            _registeredId = serviceId;
            var prefixes = string.Join(", ", _manifest.Routes.Select(r => r.Prefix));
            _logger.LogInformation(
                "Registered service '{Service}' (id={Id}) at {Address} with {RouteCount} route prefix(es) [{Prefixes}]; health={Health}; grpc={Grpc}",
                _service.Name, serviceId, _service.Address, _manifest.Routes.Count, prefixes, healthUrl, grpcUrl);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Best-effort Consul register failed for '{Service}' at {Consul}; service will still start",
                _service.Name, baseUri);
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

    private static string BuildHealthCheckUrl(Uri baseUri, string path)
    {
        var basePart = baseUri.GetLeftPart(UriPartial.Authority);
        var trimmed = path.StartsWith('/') ? path : "/" + path;
        return basePart + trimmed;
    }

    private static string NormalizeBaseUrl(Uri uri) =>
        uri.GetLeftPart(UriPartial.Authority).TrimEnd('/') + "/";

    private string ResolveGrpcUrl(Uri restBaseUri)
    {
        if (!string.IsNullOrWhiteSpace(_service.GrpcAddress)
            && Uri.TryCreate(_service.GrpcAddress, UriKind.Absolute, out var explicitGrpc))
        {
            return explicitGrpc.GetLeftPart(UriPartial.Authority);
        }

        return ConsulGrpcAddress.DeriveGrpcUrl(restBaseUri);
    }
}
