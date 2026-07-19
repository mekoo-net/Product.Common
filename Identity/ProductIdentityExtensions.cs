using Product.Common.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Product.Common.Identity;

public static class ProductIdentityExtensions
{
    /// <summary>注册 Staff permission 校验（需已 <see cref="Hosting.ProductInfrastructureExtensions.AddProductCache"/>）。</summary>
    public static IServiceCollection AddProductPlatformPermissions(this IServiceCollection services)
        => services.AddStaffPermissions();

    /// <summary>
    /// 将 Product 层 permission source 桥接到 Product.Common DI。
    /// </summary>
    public static IServiceCollection AddProductPlatformRolePermissionSource<T>(this IServiceCollection services)
        where T : class, IStaffRolePermissionSource
    {
        services.AddSingleton<T>();
        services.AddSingleton<IStaffRolePermissionSource>(sp => sp.GetRequiredService<T>());
        return services;
    }

    public static IServiceCollection AddProductAccountRolePermissionSource<T>(this IServiceCollection services)
        where T : class, IAccountRolePermissionSource
    {
        services.AddSingleton<T>();
        services.AddSingleton<IAccountRolePermissionSource>(sp => sp.GetRequiredService<T>());
        return services;
    }
}
