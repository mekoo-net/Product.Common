using Consul;
using Microsoft.Extensions.Options;

namespace Product.Common.Catalog.GatewayConsul;

public static class GatewayConsulClientFactory
{
    public static IConsulClient? TryCreateClient(GatewayConsulOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Address))
            return null;

        if (!Uri.TryCreate(options.Address, UriKind.Absolute, out var addr))
            return null;

        return CreateClient(addr, options);
    }

    public static IConsulClient CreateRequiredClient(GatewayConsulOptions options, string addressConfigPath)
    {
        if (string.IsNullOrWhiteSpace(options.Address))
        {
            throw new InvalidOperationException(
                $"{addressConfigPath} is not configured.");
        }

        if (!Uri.TryCreate(options.Address, UriKind.Absolute, out var addr))
        {
            throw new InvalidOperationException(
                $"{addressConfigPath} ('{options.Address}') is not a valid absolute URI.");
        }

        return CreateClient(addr, options);
    }

    private static IConsulClient CreateClient(Uri addr, GatewayConsulOptions options)
        => new ConsulClient(cfg =>
        {
            cfg.Address = addr;
            if (!string.IsNullOrWhiteSpace(options.Datacenter)) cfg.Datacenter = options.Datacenter;
            if (!string.IsNullOrWhiteSpace(options.Token)) cfg.Token = options.Token;
        });
}
