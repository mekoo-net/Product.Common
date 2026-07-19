using MagicOnion;

namespace Meeko.Contracts.Billing;

/// <summary>
/// Bff → Billing：充值记录管理面（list / detail）。鉴权由 Bff 完成。
/// 写操作（内部入账）通过 IBillingWalletService.CreateRechargeAsync(provider="manual") + ConfirmRechargeAsync 组合实现。
/// </summary>
public interface IBillingRechargeAdminService : IService<IBillingRechargeAdminService>
{
    UnaryResult<ListRechargesResult> ListRechargesAsync(ListRechargesQuery query);

    UnaryResult<RechargeDto?> GetRechargeAsync(long rechargeUid);

    UnaryResult<RechargeDto?> GetRechargeBySerialAsync(string serialNo);

    /// <summary>Admin 内部入账：单事务创建充值记录并给钱包加余额。</summary>
    UnaryResult<RechargeDto> CreateInternalAsync(CreateInternalRechargeCommand cmd);

    /// <summary>管理员手工确认待支付充值单入账（含三方支付掉单补录）。</summary>
    UnaryResult<RechargeDto> ConfirmManualAsync(ConfirmManualRechargeCommand cmd);
}
