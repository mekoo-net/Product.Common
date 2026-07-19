using MessagePack;

namespace Meeko.Contracts.Billing;

[MessagePackObject]
public sealed class RechargeAmountInfo
{
    [Key(0)] public required decimal Value { get; set; }
    [Key(1)] public required string Currency { get; set; }
}
