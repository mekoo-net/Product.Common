using System.Text.Json.Serialization;

namespace Product.Common.Catalog.GatewayConsul;

public sealed class ProviderCatalogWire
{
    [JsonPropertyName("providerId")]
    public string? ProviderId { get; set; }

    [JsonPropertyName("models")]
    public List<string>? Models { get; set; }
}
