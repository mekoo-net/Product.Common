namespace Meeko.Contracts.Billing;

public enum WalletTxnKind : short
{
    Recharge = 0,
    Hold = 1,
    Commit = 2,
    Release = 3,
    Refund = 4,
    Adjustment = 5,
}

public enum WalletTxnReferenceKind : short
{
    None = 0,
    Recharge = 1,
    Hold = 2,
    Manual = 3,
}

public enum RechargeStatus : short
{
    Pending = 0,
    Succeeded = 1,
    Failed = 2,
    Expired = 3,
    Refunded = 4,
}

public enum PaymentScene : short
{
    Native = 0,
    H5 = 1,
    JsApi = 2,
    App = 3,
    Pc = 4,
    Manual = 99,
}

public enum HoldStatus : short
{
    Held = 0,
    Committed = 1,
    Released = 2,
    Expired = 3,
}

public enum UsageStatus : short
{
    Settled = 0,
    Adjusted = 1,
    Voided = 2,
}

public enum ReferralWithdrawalStatus : short
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Paid = 3,
}

public enum ReferralWithdrawalMethod : short
{
    Alipay = 0,
    Bank = 1,
}

public enum VoucherDeductKind : short
{
    NoThreshold = 0,
    FullReduction = 1,
    Discount = 2,
}

public enum VoucherApplyMode : short
{
    FirstPaymentOnly = 0,
    EveryRenewal = 1,
}

public enum VoucherScopeKind : short
{
    AllProducts = 0,
    SpecificProducts = 1,
}

public enum VoucherActivityStatus : short
{
    Active = 0,
    Paused = 1,
    Ended = 2,
}

public enum VoucherValidityKind : short
{
    Absolute = 0,
    RelativeDays = 1,
}

public enum VoucherTemplateStatus : short
{
    Draft = 0,
    Active = 1,
    Paused = 2,
    Archived = 3,
}

public enum UserVoucherStatus : short
{
    Unused = 0,
    Used = 1,
    Expired = 2,
    Revoked = 3,
}

/// <summary>券余额流水类型（对称于钱包流水种类）。</summary>
public enum VoucherLedgerKind : short
{
    Issue = 0,
    Hold = 1,
    Release = 2,
    Redeem = 3,
    Refund = 4,
    Expire = 5,
    Revoke = 6,
}

public enum VoucherRedeemCodeStatus : short
{
    Unused = 0,
    Redeemed = 1,
    Disabled = 2,
}

public enum VoucherCodeBatchStatus : short
{
    Active = 0,
    Disabled = 1,
}

public enum VoucherGrantRuleStatus : short
{
    Active = 0,
    Paused = 1,
    Ended = 2,
}

public enum GrantConditionKind : short
{
    Immediate = 0,
    EventAmountAtLeast = 1,
}
