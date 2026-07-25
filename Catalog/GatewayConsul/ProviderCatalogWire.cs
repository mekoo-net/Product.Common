using System.Text.Json.Serialization;

namespace Product.Common.Catalog.GatewayConsul;

public sealed class ProviderCatalogWire
{
    [JsonPropertyName("providerId")]
    public string? ProviderId { get; set; }

    [JsonPropertyName("models")]
    public List<string>? Models { get; set; }

    /// <summary>
    /// The wire format this provider's worker natively accepts when no
    /// per-model rule matches (see <see cref="ModelIngestFormats"/>). Values are
    /// ChatTransit wire strings: "openai.chat" / "openai.responses" / "anthropic"
    /// / "gemini". Mirrors OneApi's <c>ProviderCatalog.DefaultIngestFormat</c> —
    /// see that type for the authoritative doc. Null ⇒ the provider did not
    /// declare one (legacy catalog); the edge then leaves the request untouched.
    /// </summary>
    [JsonPropertyName("defaultIngestFormat")]
    public string? DefaultIngestFormat { get; set; }

    /// <summary>
    /// Optional per-model ingest format overrides for virtual/aggregator
    /// providers whose models speak different protocols. Case-insensitive
    /// model-id prefix → wire format; longest matching prefix wins, falling back
    /// to <see cref="DefaultIngestFormat"/>. Mirrors OneApi's
    /// <c>ProviderCatalog.ModelIngestFormats</c>.
    /// </summary>
    [JsonPropertyName("modelIngestFormats")]
    public Dictionary<string, string>? ModelIngestFormats { get; set; }

    /// <summary>
    /// Resolves the ingest format for <paramref name="modelId"/>: longest
    /// matching prefix in <see cref="ModelIngestFormats"/>, else
    /// <see cref="DefaultIngestFormat"/>, else null (undeclared).
    /// </summary>
    public string? ResolveIngestFormat(string? modelId)
    {
        if (ModelIngestFormats is { Count: > 0 } && !string.IsNullOrEmpty(modelId))
        {
            string? best = null;
            var bestLen = -1;
            foreach (var (prefix, format) in ModelIngestFormats)
            {
                if (prefix.Length > bestLen
                    && modelId.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    best = format;
                    bestLen = prefix.Length;
                }
            }
            if (best != null)
                return best;
        }

        return string.IsNullOrEmpty(DefaultIngestFormat) ? null : DefaultIngestFormat;
    }
}
