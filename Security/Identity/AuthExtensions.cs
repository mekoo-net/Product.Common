using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Product.Common.Identity;

public static class AuthExtensions
{
    public static IServiceCollection AddCurrentAuth(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.TryAddScoped<ICurrentAuth, CurrentAuthAccessor>();
        return services;
    }

    public static IServiceCollection AddCurrentActor(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.TryAddScoped<ICurrentActor, CurrentActorAccessor>();
        return services;
    }
}
