using MessagePack;

namespace Meeko.Contracts.Keystone;

[MessagePackObject]
public sealed class AccountAdminListResult
{
    [Key(0)] public required AccountAdminListItem[] Items { get; set; }
    [Key(1)] public required int Total { get; set; }
}
