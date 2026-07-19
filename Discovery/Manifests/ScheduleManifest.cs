using System.Text.Json;
using System.Text.Json.Serialization;
using Product.Common.Discovery;

namespace Product.Common.Discovery.Manifests;

public static class ScheduleManifest
{
    public const string MetaKey = ConsulServiceMeta.Schedules;

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        WriteIndented = false,
    };

    public static string Encode(IEnumerable<ScheduledTaskDeclaration> schedules)
    {
        var snapshot = schedules
            .Where(s => !string.IsNullOrWhiteSpace(s.TaskKey) && !string.IsNullOrWhiteSpace(s.Cron))
            .Select(s => new ScheduledTaskDeclarationDto(
                s.TaskKey.Trim(),
                s.Cron.Trim()))
            .ToArray();

        return JsonSerializer.Serialize(snapshot, SerializerOptions);
    }

    private sealed record ScheduledTaskDeclarationDto(
        [property: JsonPropertyName("taskKey")] string TaskKey,
        [property: JsonPropertyName("cron")] string Cron);
}
