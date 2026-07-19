using Consul;

namespace Product.Common.Discovery;

/// <summary>
/// 从 Consul <see cref="ServiceEntry"/> 推导 MagicOnion gRPC 端点的纯函数集合。
/// 优先读 <see cref="ConsulServiceMeta.GrpcUrl"/>，其次 base-url 推导，最后回退到注册地址。
/// 同时被服务注册器（写 meta）与 <see cref="ConsulGrpcResolver"/>（gRPC 名称解析）复用。
/// </summary>
public static class ConsulGrpcAddress
{
    /// <summary>尝试把一个 Consul 服务实例解析为 gRPC 端点 URI（含 scheme/host/port）。</summary>
    public static bool TryResolve(ServiceEntry entry, out Uri uri)
    {
        if (entry.Service.Meta is { } meta
            && meta.TryGetValue(ConsulServiceMeta.GrpcUrl, out var grpcUrl)
            && Uri.TryCreate(NormalizeGrpcUrl(grpcUrl), UriKind.Absolute, out var grpcUri))
        {
            uri = grpcUri;
            return true;
        }

        if (entry.Service.Meta is not null
            && entry.Service.Meta.TryGetValue(ConsulServiceMeta.BaseUrl, out var baseUrl)
            && Uri.TryCreate(baseUrl, UriKind.Absolute, out var restUri)
            && Uri.TryCreate(DeriveGrpcUrl(restUri), UriKind.Absolute, out var derivedUri))
        {
            uri = derivedUri;
            return true;
        }

        if (!string.IsNullOrWhiteSpace(entry.Service.Address) && entry.Service.Port > 0
            && Uri.TryCreate(
                DeriveGrpcUrl(new Uri($"http://{entry.Service.Address}:{entry.Service.Port}/")),
                UriKind.Absolute,
                out var addrUri))
        {
            uri = addrUri;
            return true;
        }

        uri = null!;
        return false;
    }

    /// <summary>把 REST base URI 推导为 gRPC URI（约定 gRPC 端口 = REST 端口 + 1）。</summary>
    public static string DeriveGrpcUrl(Uri restBaseUri)
    {
        var builder = new UriBuilder(restBaseUri)
        {
            Port = restBaseUri.Port + 1,
        };
        return NormalizeGrpcUrl(builder.Uri.GetLeftPart(UriPartial.Authority));
    }

    private static string NormalizeGrpcUrl(string raw)
    {
        if (!Uri.TryCreate(raw, UriKind.Absolute, out var uri))
        {
            throw new InvalidOperationException($"Invalid gRPC URL '{raw}'.");
        }

        return uri.GetLeftPart(UriPartial.Authority);
    }
}
