using Platform.Common.Correlation;
using Platform.Common.Observability;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Product.Common.Auth.BackendRpc.Middleware;
using Product.Common.Hosting.Rpc;
using Serilog;

namespace Product.Common.Hosting.Bootstrap;

public sealed class ProductHostOptions
{
    public required string YamlFileName { get; init; }
    public required string ServiceName { get; init; }
    public required string ProductLabel { get; init; }
    public required Action<WebApplicationBuilder> ConfigureServices { get; init; }
    public required Func<IServiceProvider, Task> MigrateAsync { get; init; }
    public Func<WebApplication, Task>? SeedAsync { get; init; }
    public required Action<WebApplication> MapHttpEndpoints { get; init; }
    public required Action<WebApplication> MapBackendRpcEndpoints { get; init; }
}

public static class ProductHostBootstrap
{
    public static async Task RunAsync(string[] args, ProductHostOptions options)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.Sources.Clear();
#if DEBUG
            builder.Configuration.AddYamlFile(
                Path.Combine(Directory.GetCurrentDirectory(), options.YamlFileName),
                optional: false,
                reloadOnChange: true);
#else
            builder.Configuration.AddYamlFile(
                Path.Combine("/config", options.YamlFileName),
                optional: false,
                reloadOnChange: true);
#endif

            builder.AddPlatformObservability(o => o.ServiceName = options.ServiceName);
            builder.UsePlatformSerilog();

            options.ConfigureServices(builder);

            var app = builder.Build();

            Log.Information("Applying {Product} schema migrations...", options.ProductLabel);
            await options.MigrateAsync(app.Services);
            Log.Information("{Product} schema migrations applied.", options.ProductLabel);

            if (options.SeedAsync is not null)
            {
                await options.SeedAsync(app);
                Log.Information("{Product} settings seed complete.", options.ProductLabel);
            }

            app.UseCorrelationId();
            app.UseAuthentication();
            app.UseAuthorization();
            ProductRpcHosting.UseBackendRpcAuthPipeline<BackendRpcAuthMiddleware>(app);

            options.MapHttpEndpoints(app);
            options.MapBackendRpcEndpoints(app);
            ProductRpcHosting.LogConfiguredKestrelEndpoints(app.Configuration);

            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "{Product} host terminated unexpectedly during startup.", options.ProductLabel);
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
