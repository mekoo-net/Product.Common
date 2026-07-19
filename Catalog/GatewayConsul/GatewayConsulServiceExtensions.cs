using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Product.Common.Catalog.GatewayConsul;

public static class GatewayConsulServiceExtensions
{
    public static IServiceCollection AddGatewayConsul(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
    {
        services.AddOptions<GatewayConsulOptions>()
            .Bind(configuration.GetSection(sectionName));
        services.AddSingleton<GatewayConsulConnection>();
        return services;
    }
}
