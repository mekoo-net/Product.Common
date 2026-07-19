using MessagePack;

namespace Meeko.Contracts.Billing;

[MessagePackObject]
public sealed class BillFailureInfo
{
    [Key(0)] public required string Code { get; set; }
}
