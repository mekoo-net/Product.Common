using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Common.Discovery;

namespace Product.Common.Discovery;

public static class ProductConsulRegistrationExtensions
{
    /// <summary>
    /// Product 服务向 Meeko Consul 注册（含 route/product manifest）。须在 AddServiceDiscovery 之后调用。
    /// </summary>
    public static WebApplicationBuilder AddProductConsulRegistration(this WebApplicationBuilder builder)
    {
        var serviceName = builder.Configuration
            .GetSection(DiscoveryOptions.SectionName)
            .GetValue<string?>("Service:Name");

        if (!string.IsNullOrWhiteSpace(serviceName))
            builder.Services.AddHostedService<ConsulServiceRegistrar>();

        return builder;
    }
}
