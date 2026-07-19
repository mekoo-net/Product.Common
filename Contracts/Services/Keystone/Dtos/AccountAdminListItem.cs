using MessagePack;

namespace Meeko.Contracts.Keystone;

[MessagePackObject]
public sealed class AccountAdminListItem
{
    [Key(0)] public required long Uid { get; set; }
    [Key(1)] public required string Type { get; set; }
    [Key(2)] public required string DisplayName { get; set; }
    [Key(3)] public required string Status { get; set; }

    /// <summary>账户等级（1..5）。Key 顺延以兼容旧消息，声明位置就近 Status。</summary>
    [Key(8)] public int Tier { get; set; }

    /// <summary>该账户已持有的成就/徽章 code（用于列表筛选与展示）。</summary>
    [Key(9)] public string[] AchievementCodes { get; set; } = [];

    [Key(4)] public AccountOwnerInfo? Owner { get; set; }

    /// <summary>邀请人（返利关系）联系信息：展示名 / 邮箱 / 手机；自然注册为 null。Key 顺延以兼容旧消息，声明位置就近 Owner。</summary>
    [Key(10)] public AccountContactDto? Inviter { get; set; }

    [Key(5)] public int IamUserCount { get; set; }
    [Key(6)] public DateTime CreatedAtUtc { get; set; }

    /// <summary>最近活跃信息（时间 + 来源 IP）；账户从未登录时为 null。</summary>
    [Key(11)] public AccountActiveInfo? Active { get; set; }
}
