using Consul;
using Microsoft.Extensions.Options;

namespace Product.Common.Catalog.GatewayConsul;

public sealed class GatewayConsulConnection : IDisposable
{
    public IConsulClient? Client { get; }

    public GatewayConsulConnection(IOptions<GatewayConsulOptions> options)
        => Client = GatewayConsulClientFactory.TryCreateClient(options.Value);

    public void Dispose() => (Client as IDisposable)?.Dispose();
}
