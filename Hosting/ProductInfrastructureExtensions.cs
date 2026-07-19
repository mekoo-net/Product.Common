using Platform.Common.Caching;
using Platform.Common.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Product.Common.Hosting;

/// <summary>Product 层对 Platform.Common Hosting 能力的统一入口（Product 项目禁止直接引用 Platform.Common.Hosting）。</summary>
public static class ProductInfrastructureExtensions
{
    public static IWebHostBuilder ConfigureProductPlatformKestrel(this IWebHostBuilder host)
        => host.ConfigurePlatformKestrel();

    public static string? GetProductDbConnectionString(this IConfiguration configuration)
        => configuration.GetDbConnectionString();

    public static string GetRequiredProductDbConnectionString(this IConfiguration configuration)
        => configuration.GetRequiredDbConnectionString();

    public static string? GetProductRedisConnectionString(this IConfiguration configuration)
        => configuration.GetRedisConnectionString();

    public static string GetRequiredProductRedisConnectionString(this IConfiguration configuration)
        => configuration.GetRequiredRedisConnectionString();

    public static IServiceCollection AddProductCache(
        this IServiceCollection services,
        string serviceName,
        IConfiguration configuration)
        => services.AddPlatformCache(serviceName, configuration);
}
