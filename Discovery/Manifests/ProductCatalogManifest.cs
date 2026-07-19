using System.Text.Json;
using System.Text.Json.Serialization;
using Product.Common.Discovery;

namespace Product.Common.Discovery.Manifests;

public static class ProductCatalogManifest
{
    public const string MetaKey = ConsulServiceMeta.Products;

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        WriteIndented = false,
    };

    public static string Encode(IEnumerable<ProductDeclaration> products)
    {
        var snapshot = products
            .Where(p => !string.IsNullOrWhiteSpace(p.Code))
            .Select(p => new ProductDeclarationDto(
                p.Code.Trim(),
                string.IsNullOrWhiteSpace(p.DisplayName) ? p.Code.Trim() : p.DisplayName.Trim()))
            .ToArray();

        return JsonSerializer.Serialize(snapshot, SerializerOptions);
    }

    private sealed record ProductDeclarationDto(
        [property: JsonPropertyName("code")] string Code,
        [property: JsonPropertyName("displayName")] string DisplayName);
}
