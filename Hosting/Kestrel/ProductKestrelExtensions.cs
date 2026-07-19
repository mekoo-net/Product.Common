using Microsoft.Extensions.Configuration;

namespace Product.Common.Hosting.Kestrel;

/// <summary>
/// Product 三端口 Kestrel 约定：Http / InternalRpc / BackendRpc。
/// 端口仅在各产品 yaml 的 <c>Kestrel:Endpoints</c> 中定义，代码禁止 magic number。
/// </summary>
public static class ProductKestrelExtensions
{
    public static class EndpointNames
    {
        public const string Http = "Http";
        public const string InternalRpc = "InternalRpc";
        public const string BackendRpc = "BackendRpc";
    }

    public static int GetKestrelEndpointPort(this IConfiguration configuration, string endpointName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(endpointName);

        var url = configuration[$"Kestrel:Endpoints:{endpointName}:Url"];
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new InvalidOperationException(
                $"Kestrel endpoint '{endpointName}' is not configured (Kestrel:Endpoints:{endpointName}:Url).");
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            throw new InvalidOperationException(
                $"Kestrel endpoint '{endpointName}' Url is invalid: '{url}'.");
        }

        if (uri.Port <= 0)
        {
            throw new InvalidOperationException(
                $"Kestrel endpoint '{endpointName}' Url must include an explicit port: '{url}'.");
        }

        return uri.Port;
    }
}
