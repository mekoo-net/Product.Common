using MessagePack;

namespace Meeko.Contracts.Billing;

[MessagePackObject]
public sealed class BillReversalInfo
{
    [Key(0)] public DateTime AtUtc { get; set; }
    [Key(1)] public long? ByIamUserUid { get; set; }
    [Key(2)] public string? Code { get; set; }
    [Key(3)] public decimal RefundedAmount { get; set; }
}
