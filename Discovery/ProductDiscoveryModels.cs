namespace Product.Common.Discovery;

public sealed class ProductRegistration
{
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

public sealed class ProductScheduleRegistration
{
    public string TaskKey { get; set; } = string.Empty;
    public string Cron { get; set; } = string.Empty;
}

public static class ProductDiscoveryMapping
{
    public static ProductDeclaration ToDeclaration(ProductRegistration registration) => new()
    {
        Code = registration.Code,
        DisplayName = registration.DisplayName,
    };

    public static ScheduledTaskDeclaration ToSchedule(ProductScheduleRegistration registration) => new()
    {
        TaskKey = registration.TaskKey,
        Cron = registration.Cron,
    };

    public static bool TryResolveGrpcAddress(Consul.ServiceEntry entry, out Uri uri)
        => ConsulGrpcAddress.TryResolve(entry, out uri);
}
