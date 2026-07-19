using MessagePack;

namespace Meeko.Contracts.Billing;

[MessagePackObject]
public sealed class ListRechargesResult
{
    [Key(0)] public required RechargeDto[] Items { get; set; }
    [Key(1)] public required int Total { get; set; }
}
