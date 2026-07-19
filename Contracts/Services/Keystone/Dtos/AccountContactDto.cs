using System.Text.Json.Serialization;
using Platform.Common.Web;
using MessagePack;

namespace Meeko.Contracts.Keystone;

/// <summary>
/// 账户身份 + 联系信息的统一嵌套投影：展示名 + Owner 联系邮箱 / 手机 + 账户类型。
/// 全平台凡「接口返回的账户身份/联系信息」一律复用本类型，禁止再展平成 ownerXxx / inviterXxx。
/// 供 Bff enrich 充值 / 账单 / 返利 / 券活动等只含 AccountUid 的流水。
/// </summary>
[MessagePackObject]
public sealed class AccountContactDto
{
    /// <summary>账户 UID。REST 序列化为 string（long → string），RPC 走 MessagePack 原生 long。</summary>
    [Key(0)]
    [JsonConverter(typeof(LongToStringConverter))]
    public required long Uid { get; set; }
    [Key(1)] public string? DisplayName { get; set; }
    [Key(2)] public string? Email { get; set; }
    [Key(3)] public string? Phone { get; set; }

    /// <summary>账户类型（personal / organization），供需要展示账户身份的列表 enrich。</summary>
    [Key(4)] public string? AccountType { get; set; }
}
