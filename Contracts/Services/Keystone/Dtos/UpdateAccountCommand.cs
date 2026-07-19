using MessagePack;

namespace Meeko.Contracts.Keystone;

[MessagePackObject]
public sealed class UpdateAccountCommand
{
    [Key(0)] public required long AccountUid { get; set; }
    [Key(1)] public string? DisplayName { get; set; }
    [Key(2)] public string? Description { get; set; }
}
