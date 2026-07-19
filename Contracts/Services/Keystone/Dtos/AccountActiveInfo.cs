using MessagePack;

namespace Meeko.Contracts.Keystone;

/// <summary>
/// 账户最近活跃信息：取该账户最新一条登录会话（<c>Session</c>）的活动时间与来源 IP。
/// 账户从未登录（无会话）时整个对象为 null。
/// </summary>
[MessagePackObject]
public sealed class AccountActiveInfo
{
    /// <summary>最近活跃时间（最新会话的 LastActivityAt，UTC）。</summary>
    [Key(0)] public DateTime? LastActiveAtUtc { get; set; }

    /// <summary>最近活跃来源 IP（最新会话的 IpAddress）。</summary>
    [Key(1)] public string? LastActiveIp { get; set; }
}
