using Microsoft.Extensions.DependencyInjection;

namespace Product.Common.PlatformServices;

public static class ProductPlatformServiceExtensions
{
    /// <summary>注册 Billing 平台服务组（Product 需要时才调用）。</summary>
    public static IServiceCollection AddBillingPlatformService(this IServiceCollection services)
        => PlatformServiceCollectionExtensions
            .AddBillingPlatformService(services)
            .AddSingleton<Billing.PlatformBillingFacade>();

    /// <summary>注册 Keystone 平台服务组（Product 需要时才调用）。</summary>
    public static IServiceCollection AddKeystonePlatformService(this IServiceCollection services)
        => PlatformServiceCollectionExtensions
            .AddKeystonePlatformService(services)
            .AddSingleton<Keystone.PlatformKeystoneAccountsFacade>();

    /// <summary>注册 Storage 平台服务组（Product 需要时才调用）。</summary>
    public static IServiceCollection AddStoragePlatformService(this IServiceCollection services)
        => PlatformServiceCollectionExtensions.AddStoragePlatformService(services);
}
