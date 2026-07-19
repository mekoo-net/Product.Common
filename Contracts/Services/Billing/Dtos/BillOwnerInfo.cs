using Meeko.Contracts.Keystone;
using MessagePack;

namespace Meeko.Contracts.Billing;

[MessagePackObject]
public sealed class BillOwnerInfo
{
    [Key(0)] public required long AccountUid { get; set; }

    /// <summary>账户身份与联系信息（展示名 / 邮箱 / 手机 / 类型）；由 BFF 按 uid 批量 enrich，统一嵌套 <see cref="AccountContactDto"/>。</summary>
    [Key(1)] public AccountContactDto? Contact { get; set; }
}
