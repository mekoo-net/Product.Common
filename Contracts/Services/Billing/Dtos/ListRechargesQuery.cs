using MessagePack;

namespace Meeko.Contracts.Billing;

[MessagePackObject]
public sealed class ListRechargesQuery
{
    [Key(0)] public int Page { get; set; } = 1;
    [Key(1)] public int PageSize { get; set; } = 20;
    [Key(2)] public long? AccountUid { get; set; }
    [Key(3)] public string? Provider { get; set; }
    [Key(4)] public string? Status { get; set; }
    [Key(5)] public DateTime? FromUtc { get; set; }
    [Key(6)] public DateTime? ToUtc { get; set; }
}
