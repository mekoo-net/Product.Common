using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Product.Common.Identity;

public static class CurrentUserExtensions
{
    /// <summary>
    /// 注册基于 Gateway 注入的 header 解析的 ICurrentUser。下游服务（Bff/Billing/Notice/Jobs）使用。
    /// </summary>
    public static IServiceCollection AddCurrentUser(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.TryAddScoped<ICurrentUser, CurrentUserAccessor>();
        return services;
    }

    /// <summary>
    /// 注册基于 Gateway 注入的 header 解析的 ICurrentActor（v1.2）。下游业务服务用。
    /// </summary>
    public static IServiceCollection AddCurrentActor(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.TryAddScoped<ICurrentActor, CurrentActorAccessor>();
        return services;
    }
}
