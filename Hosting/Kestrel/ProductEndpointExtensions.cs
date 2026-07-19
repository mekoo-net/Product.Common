using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Product.Common.Hosting.Kestrel;

/// <summary>
/// Product 多端口 endpoint 路由：按 TCP 监听端口分流（类比 nginx server block）。
/// </summary>
public static class ProductEndpointExtensions
{
    public static RouteGroupBuilder RequireLocalPort(this RouteGroupBuilder group, int localPort)
    {
        group.AddEndpointFilter(async (context, next) =>
        {
            if (context.HttpContext.Connection.LocalPort != localPort)
                return Microsoft.AspNetCore.Http.Results.NotFound();

            return await next(context);
        });
        return group;
    }
}
