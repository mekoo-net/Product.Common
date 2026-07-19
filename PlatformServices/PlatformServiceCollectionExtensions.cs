using Microsoft.Extensions.DependencyInjection;

namespace Product.Common.PlatformServices;

public static class PlatformServiceCollectionExtensions
{
    public static IServiceCollection AddBillingPlatformService(this IServiceCollection services)
        => services.AddSingleton<BillingPlatformService>();

    public static IServiceCollection AddKeystonePlatformService(this IServiceCollection services)
        => services.AddSingleton<KeystonePlatformService>();

    public static IServiceCollection AddStoragePlatformService(this IServiceCollection services)
        => services.AddSingleton<StoragePlatformService>();
}
