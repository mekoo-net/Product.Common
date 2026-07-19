using Meeko.Contracts.Keystone;
using MessagePack;

namespace Meeko.Contracts.Billing;

[MessagePackObject]
public sealed class BillOperatorInfo
{
    [Key(0)] public required long AccountUid { get; set; }

    /// <summary>区分席位时使用；当前 WalletTxn 没存 IAM 用户上下文，返回 null。</summary>
    [Key(1)] public long? IamUserUid { get; set; }

    /// <summary>操作者账户身份与联系信息；由 BFF 按 uid 批量 enrich，统一嵌套 <see cref="AccountContactDto"/>。</summary>
    [Key(2)] public AccountContactDto? Contact { get; set; }
}
