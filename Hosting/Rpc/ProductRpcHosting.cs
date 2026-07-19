using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Product.Common.Hosting.Kestrel;
using Serilog;

namespace Product.Common.Hosting.Rpc;

/// <summary>
/// Product 三端口 RPC 托管：InternalRpc（平台内）与 BackendRpc（独立运行时回调）。
/// </summary>
public static class ProductRpcHosting
{
    public static void UseBackendRpcAuthPipeline<TMiddleware>(WebApplication app)
        where TMiddleware : class
    {
        var port = app.Configuration.GetKestrelEndpointPort(ProductKestrelExtensions.EndpointNames.BackendRpc);
        app.UseWhen(
            ctx => ctx.Connection.LocalPort == port,
            branch => branch.UseMiddleware<TMiddleware>());
    }

    public static RouteGroupBuilder MapBackendRpcGroup(WebApplication app)
    {
        var port = app.Configuration.GetKestrelEndpointPort(ProductKestrelExtensions.EndpointNames.BackendRpc);
        return app.MapGroup(string.Empty).RequireLocalPort(port);
    }

    public static RouteGroupBuilder MapInternalRpcGroup(WebApplication app)
    {
        var port = app.Configuration.GetKestrelEndpointPort(ProductKestrelExtensions.EndpointNames.InternalRpc);
        return app.MapGroup(string.Empty).RequireLocalPort(port);
    }

    public static void LogConfiguredKestrelEndpoints(IConfiguration configuration)
    {
        foreach (var endpoint in configuration.GetSection("Kestrel:Endpoints").GetChildren())
        {
            var url = endpoint["Url"];
            if (!string.IsNullOrWhiteSpace(url))
                Log.Information("Kestrel endpoint {Name}: {Url}", endpoint.Key, url);
        }
    }
}
