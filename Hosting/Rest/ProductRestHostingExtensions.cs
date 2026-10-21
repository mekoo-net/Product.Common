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
        builder.AddProductConsulRegistration(serviceKey);
        return builder;
    }

    public static WebApplicationBuilder AddProductStaffSurface(this WebApplicationBuilder builder)
    {
        builder.Services.AddCurrentActor();
        return builder;
    }

    public static WebApplication MapProductHttpEndpoints(this WebApplication app)
    {
        app.MapControllers();
        app.MapHealthChecks();
        return app;
    }
}
