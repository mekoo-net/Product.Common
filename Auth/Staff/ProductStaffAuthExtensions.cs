using Microsoft.Extensions.DependencyInjection;
using Platform.Common.Identity;
using Product.Common.Identity;

namespace Product.Common.Auth.Staff;

public static class ProductStaffAuthExtensions
{
    public static IServiceCollection AddProductStaffPermissionSources(this IServiceCollection services)
    {
        services.AddProductPlatformRolePermissionSource<PlatformStaffPermissionSource>();
        services.AddProductAccountRolePermissionSource<PlatformAccountPermissionSource>();
        return services;
    }

    public static IServiceCollection AddProductPermissionCatalogRegistration(
        this IServiceCollection services,
        PermissionCatalogRegistration registration)
    {
        services.AddSingleton(registration);
        services.AddHostedService<PermissionCatalogRegistrationHostedService>();
        return services;
    }
}
