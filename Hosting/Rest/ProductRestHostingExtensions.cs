using Product.Common.Auth;
using Product.Common.Discovery;
using Product.Common.Identity;
using Platform.Common.Discovery;
using Platform.Common.HealthChecks;
using Platform.Common.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Product.Common.Hosting.Rest;

public static class ProductRestHostingExtensions
{
    public static WebApplicationBuilder AddProductRestSurface(
        this WebApplicationBuilder builder,
        Action<DiscoveryOptions>? configureDiscovery = null)
    {
        builder.Services.AddGatewayForwardedAuth();
        builder.Services.AddCurrentUser();
        builder.Services.AddPlatformJsonOptions();
        builder.Services.AddControllers();
        builder.AddServiceDiscovery();
        builder.AddProductServiceClients();
        builder.AddProductConsulRegistration();
        if (configureDiscovery is not null)
            builder.Services.PostConfigure(configureDiscovery);
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
