using Consul;
using Microsoft.Extensions.Logging;

namespace Product.Common.Discovery;

/// <summary>
/// 从 Consul <see cref="ServiceEntry"/> 推导对外 HTTP base URL 的纯函数集合。
/// 优先 <see cref="ConsulServiceMeta.PublicUrl"/>，其次 <see cref="ConsulServiceMeta.BaseUrl"/>，最后回退注册地址。
/// </summary>
public static class ConsulHttpAddress
{
    public static bool TryResolve(ServiceEntry entry, out Uri uri)
    {
        if (entry.Service.Meta is { } meta)
        {
            if (meta.TryGetValue(ConsulServiceMeta.PublicUrl, out var publicUrl)
                && Uri.TryCreate(publicUrl, UriKind.Absolute, out var publicUri))
            {
                uri = publicUri;
                return true;
            }

            if (meta.TryGetValue(ConsulServiceMeta.BaseUrl, out var baseUrl)
                && Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri))
            {
                uri = baseUri;
                return true;
            }
        }

        if (!string.IsNullOrWhiteSpace(entry.Service.Address) && entry.Service.Port > 0
            && Uri.TryCreate($"http://{entry.Service.Address}:{entry.Service.Port}/", UriKind.Absolute, out var addrUri))
        {
            uri = addrUri;
            return true;
        }

        uri = null!;
        return false;
    }
}

internal sealed class ConsulHttpEndpointResolver : IHttpEndpointResolver
{
    private readonly IConsulClient _consul;
    private readonly ILogger<ConsulHttpEndpointResolver> _logger;

    public ConsulHttpEndpointResolver(IConsulClient consul, ILogger<ConsulHttpEndpointResolver> logger)
    {
        _consul = consul;
        _logger = logger;
    }

    public async Task<Uri> ResolvePublicUrlAsync(string serviceName, CancellationToken cancellationToken = default)
    {
        QueryResult<ServiceEntry[]> result;
        try
        {
            result = await _consul.Health.Service(serviceName, string.Empty, passingOnly: true, cancellationToken);
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Consul health query for '{serviceName}' failed.", ex);
        }

        var entries = result.Response ?? [];
        if (entries.Length == 0)
        {
            throw new InvalidOperationException(
                $"No healthy Consul service instances found for '{serviceName}'.");
        }

        foreach (var entry in entries)
        {
            if (ConsulHttpAddress.TryResolve(entry, out var uri))
            {
                _logger.LogDebug(
                    "Resolved public HTTP URL for '{Service}' -> {Url}", serviceName, uri.GetLeftPart(UriPartial.Authority));
                return uri;
            }
        }

        throw new InvalidOperationException(
            $"Healthy Consul instances for '{serviceName}' lack resolvable HTTP metadata.");
    }
}
