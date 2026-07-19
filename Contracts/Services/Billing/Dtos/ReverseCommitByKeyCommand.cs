using MessagePack;

namespace Meeko.Contracts.Billing;

[MessagePackObject]
public sealed class ReverseCommitByKeyCommand
{
    [Key(0)] public required string IdempotencyKey { get; set; }
    [Key(1)] public required string Code { get; set; }
    [Key(2)] public string? Note { get; set; }
    [Key(3)] public long? OperatorIamUserUid { get; set; }
}
