using MessagePack;

namespace Meeko.Contracts.Keystone;

[MessagePackObject]
public sealed class AccountAchievementDto
{
    [Key(0)] public required string Code { get; set; }
    [Key(1)] public required string Name { get; set; }
    [Key(2)] public required string Description { get; set; }
    [Key(3)] public required string Icon { get; set; }
    [Key(4)] public string? Image { get; set; }
    [Key(5)] public DateTime GrantedAtUtc { get; set; }
}

[MessagePackObject]
public sealed class GrantAchievementCommand
{
    [Key(0)] public required long AccountUid { get; set; }
    [Key(1)] public required string Code { get; set; }
    [Key(2)] public long? OperatorStaffUid { get; set; }
}

[MessagePackObject]
public sealed class RevokeAchievementCommand
{
    [Key(0)] public required long AccountUid { get; set; }
    [Key(1)] public required string Code { get; set; }
}
