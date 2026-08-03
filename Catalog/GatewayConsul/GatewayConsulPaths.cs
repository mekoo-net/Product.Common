namespace Product.Common.Catalog.GatewayConsul;

/// <summary>
/// Consul KV path helpers for provider catalog blobs published by OneApi
/// <c>ProviderCatalogPublisher</c> at <c>{prefix}/providers/{providerId}/catalog</c>.
/// </summary>
public static class GatewayConsulPaths
{
    public static string NormalizeKvPrefix(string? kvPrefix) =>
        kvPrefix?.Trim().Trim('/') ?? "";

    public static string ProvidersListPrefix(string? kvPrefix)
    {
        var p = NormalizeKvPrefix(kvPrefix);
        return string.IsNullOrEmpty(p) ? "providers/" : $"{p}/providers/";
    }

    public static string ProviderCatalogKey(string? kvPrefix, string providerId)
    {
        var p = NormalizeKvPrefix(kvPrefix);
        return string.IsNullOrEmpty(p)
            ? $"providers/{providerId}/catalog"
            : $"{p}/providers/{providerId}/catalog";
    }
}
