using System.Text.Json;
using System.Text.Json.Serialization;
using Product.Common.Discovery;

namespace Product.Common.Discovery.Manifests;

/// <summary>
/// 路由 manifest 在 Consul Service Meta 中的编码协议（与 Meeko 平台 wire 格式一致）。
/// </summary>
public static class ServiceRouteManifest
{
    public const string MetaKey = ConsulServiceMeta.Routes;
    public const string BaseUrlMetaKey = ConsulServiceMeta.BaseUrl;
    public const string PublicUrlMetaKey = ConsulServiceMeta.PublicUrl;
    public const string GrpcUrlMetaKey = ConsulServiceMeta.GrpcUrl;

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        WriteIndented = false,
    };

    public static string Encode(IEnumerable<RouteManifest> routes)
    {
        var snapshot = routes
            .Where(r => !string.IsNullOrWhiteSpace(r.Prefix))
            .Select(r => new RouteManifestDto(
                r.Prefix.Trim(),
                string.IsNullOrWhiteSpace(r.Policy) ? "Default" : r.Policy.Trim(),
                r.StripPrefix))
            .ToArray();

        return JsonSerializer.Serialize(snapshot, SerializerOptions);
    }

    private sealed record RouteManifestDto(
        [property: JsonPropertyName("prefix")] string Prefix,
        [property: JsonPropertyName("policy")] string Policy,
        [property: JsonPropertyName("stripPrefix")] bool StripPrefix);
}
