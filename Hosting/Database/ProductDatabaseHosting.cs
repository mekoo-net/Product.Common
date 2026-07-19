using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Product.Common.Hosting.Database;

public static class ProductDatabaseHosting
{
    public static async Task MigrateAsync<TContext>(
        IServiceProvider services,
        string productLabel,
        CancellationToken cancellationToken = default)
        where TContext : DbContext
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TContext>();
        if (!db.Database.IsRelational())
            return;

        var pending = (await db.Database.GetPendingMigrationsAsync(cancellationToken)).ToArray();
        if (pending.Length > 0)
        {
            Log.Information(
                "{Product}: applying {Count} pending migration(s): {Migrations}",
                productLabel,
                pending.Length,
                string.Join(", ", pending));
        }

        await db.Database.MigrateAsync(cancellationToken);
        Log.Information("{Product}: database migrate complete", productLabel);
    }
}
