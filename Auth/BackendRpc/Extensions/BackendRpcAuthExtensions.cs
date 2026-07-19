using Microsoft.Extensions.DependencyInjection;
using Product.Common.Auth.BackendRpc.Options;

namespace Product.Common.Auth.BackendRpc.Extensions;

public static class BackendRpcAuthExtensions
{
    public static IServiceCollection AddBackendRpcAuth(
        this IServiceCollection services,
        Action<BackendRpcAuthOptions> configure)
    {
        services.AddOptions<BackendRpcAuthOptions>().Configure(configure);
        return services;
    }
}
