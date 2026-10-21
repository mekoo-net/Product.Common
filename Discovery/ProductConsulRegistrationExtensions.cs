using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Platform.Common.Discovery;

namespace Product.Common.Discovery;

public static class ProductConsulRegistrationExtensions
{
    /// <summary>
    /// Product 服务以 <paramref name="serviceKey"/> 向 Meeko Consul 注册（含 route/product manifest）。
    /// 须在 AddServiceDiscovery 之后调用。
    /// </summary>
    /// <param name="serviceKey">本服务的 <see cref="ServiceKeys"/>，调用方按它找实例。</param>
    public static WebApplicationBuilder AddProductConsulRegistration(this WebApplicationBuilder builder, string serviceKey)
    {
        builder.Services.AddSingleton(LocalServiceInstance.Resolve(serviceKey, builder.Configuration));
        builder.Services.AddHostedService<ConsulServiceRegistrar>();

        return builder;
    }
}
