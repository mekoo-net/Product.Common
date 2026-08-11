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

    /// <summary>
    /// InternalRpc 端口的鉴权管线。与 <see cref="UseBackendRpcAuthPipeline{TMiddleware}"/> 同形，
    /// 但那把 key 是签给外部运行时的，不能共用——见 <c>InternalRpcAuthOptions</c>。
    ///
    /// <para>挂在**端口**上而不是单个服务上：那个端口整体就是管理面（账单查询、供应商管理、
    /// 定时任务触发都在上面），逐服务加注解迟早会漏掉新加的那个。</para>
    /// </summary>
    public static void UseInternalRpcAuthPipeline<TMiddleware>(WebApplication app)
        where TMiddleware : class
    {
        var port = app.Configuration.GetKestrelEndpointPort(ProductKestrelExtensions.EndpointNames.InternalRpc);
        app.UseWhen(
            ctx => ctx.Connection.LocalPort == port,
            branch => branch.UseMiddleware<TMiddleware>());
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
