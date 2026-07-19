using MessagePack;

namespace Meeko.Contracts.Billing;

[MessagePackObject]
public sealed class RechargeOperatorInfo
{
    [Key(0)] public required long IamUserUid { get; set; }

    /// <summary>操作人展示名；由 BFF 按 uid enrich。</summary>
    [Key(1)] public string? DisplayName { get; set; }
}
