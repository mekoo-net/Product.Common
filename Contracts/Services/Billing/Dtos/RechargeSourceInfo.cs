using MessagePack;

namespace Meeko.Contracts.Billing;

[MessagePackObject]
public sealed class RechargeSourceInfo
{
    [Key(0)] public required string Provider { get; set; }
    [Key(1)] public required int Scene { get; set; }
    [Key(2)] public required string RefNo { get; set; }

    [Key(3)] public string? ProductCode { get; set; }
}
