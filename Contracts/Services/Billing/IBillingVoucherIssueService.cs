using MagicOnion;

namespace Meeko.Contracts.Billing;

/// <summary>
/// 产品 → Billing：按券模板给账户发券（活动奖励等产品自己的发券场景）。
/// 券模板仍在 Billing 管理台维护；产品只能挑「适用范围包含本产品」的模板来发。
/// </summary>
public interface IBillingVoucherIssueService : IService<IBillingVoucherIssueService>
{
    /// <summary>可发的券模板：上架中、适用范围包含该产品（或全部产品）、发行总量还有余量。供产品管理台选模板。</summary>
    UnaryResult<IssuableVoucherTemplateDto[]> ListIssuableTemplatesAsync(string productCode);

    /// <summary>按 id 取模板（任何状态），供产品展示已选模板的名称与当前状态。不存在的 id 不在返回集中。</summary>
    UnaryResult<IssuableVoucherTemplateDto[]> GetTemplatesAsync(long[] templateIds);

    /// <summary>
    /// 发一张券。幂等：同一产品、同一 <see cref="IssueProductVoucherCommand.IdempotencyKey"/> 只发一张，重复调用返回同一张。
    /// 业务上发不出（模板下架、发完、超过模板的每人限领、不适用于该产品）返回 <c>Success=false</c>，确定没发；
    /// 调用抛异常则结果未知，调用方须用同一个幂等键重试。
    /// </summary>
    UnaryResult<IssueProductVoucherResult> IssueAsync(IssueProductVoucherCommand cmd);
}
