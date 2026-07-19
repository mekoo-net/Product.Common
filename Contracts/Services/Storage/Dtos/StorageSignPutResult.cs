using MessagePack;

namespace Meeko.Contracts.Storage.Dtos;

[MessagePackObject]
public sealed class StorageSignPutResult
{
    [Key(0)] public string? UploadUrl { get; set; }
    [Key(1)] public string UploadMethod { get; set; } = "PUT";
    [Key(2)] public Dictionary<string, string> UploadHeaders { get; set; } = new();
    [Key(3)] public string? FinalUrl { get; set; }
    [Key(4)] public string StorageKey { get; set; } = string.Empty;
    [Key(5)] public long ExpiresAtMs { get; set; }
}
