using Platform.Common.Discovery;
using Platform.Common.HealthChecks;
using Platform.Common.Web;
using Product.Common.Auth;
using Product.Common.Discovery;
using Product.Common.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Product.Common.Hosting.Rest;

public static class ProductRestHostingExtensions
{
    /// <summary>
    /// 产品服务对外的 REST 面：网关转发的身份、控制器、Consul 客户端栈，以及以 <paramref name="serviceKey"/>
    /// 注册进 Consul（<see cref="RoutedServiceRegistration"/>，和平台服务同一种注册）。
    /// 计费产品、定时任务由宿主 PostConfigure <see cref="ServiceDiscoveryOptions"/> 声明。
    /// </summary>
    /// <param name="serviceKey">本服务在 Consul 里的 <see cref="ServiceKeys"/>，写死在宿主代码里。</param>
    public static WebApplicationBuilder AddProductRestSurface(this WebApplicationBuilder builder, string serviceKey)
    {
        builder.Services.AddGatewayForwardedAuth();
        builder.Services.AddCurrentAuth();
        builder.Services.AddCurrentActor();
        builder.Services.AddPlatformJsonOptions();
        builder.Services.AddControllers();
        builder.AddServiceDiscovery();
        builder.AddProductServiceClients();
        builder.AddRoutedServiceRegistration(serviceKey);
        return builder;
    }

    public static WebApplication MapProductHttpEndpoints(this WebApplication app)
    {
        app.MapControllers();
        app.MapHealthChecks();
        return app;
    }
}
