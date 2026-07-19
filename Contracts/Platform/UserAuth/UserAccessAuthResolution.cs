using MessagePack;

namespace Meeko.Contracts.UserAuth;

[MessagePackObject]
public sealed class UserAccessAuthResolution
{
    [Key(0)] public required long AccountUid { get; init; }
    [Key(1)] public long? IamUserUid { get; init; }
    [Key(2)] public string? Actor { get; init; }
    [Key(3)] public string? Role { get; init; }
    [Key(4)] public long? StaffUid { get; init; }
    [Key(5)] public string? AccountType { get; init; }
}
