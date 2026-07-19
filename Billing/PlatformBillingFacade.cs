using Meeko.Contracts.Billing;
using Product.Common.PlatformServices;

namespace Product.Common.Billing;

public sealed record PlatformHoldRequest(
    long AccountUid,
    string ProductCode,
    decimal EstimateAmount,
    TimeSpan? Ttl,
    string IdempotencyKey);

public sealed record PlatformHoldResult(
    bool Success,
    long? HoldId,
    decimal? AvailableAfter,
    string? FailureCode,
    string? FailureMessage);

public sealed record PlatformCommitHoldResult(
    bool Success,
    string? BillSerialNo,
    string? FailureCode,
    string? FailureMessage);

public sealed record PlatformCommitBillLookup(
    string IdempotencyKey,
    string BillId,
    string Status,
    DateTime? ReversedAtUtc,
    string? ReversedCode);

public sealed record PlatformInternalRechargeRequest(
    long AccountUid,
    decimal Amount,
    string Source,
    string? Note,
    long? OperatorUid,
    string? IdempotencyKey,
    string? ProductCode);

public sealed record PlatformReverseCommitRequest(
    string IdempotencyKey,
    string Code,
    string? Note,
    long? OperatorIamUserUid);

public sealed record PlatformReverseCommitResult(
    bool Success,
    string? FailureCode,
    string? FailureMessage);

public sealed record PlatformWalletSnapshot(decimal Available);

public sealed class PlatformBillingFacade
{
    private readonly BillingPlatformService _billing;

    public PlatformBillingFacade(BillingPlatformService billing) => _billing = billing;

    public async Task<PlatformHoldResult> TryHoldAsync(PlatformHoldRequest request)
    {
        var hold = await _billing.Metering.TryHoldAsync(new HoldRequest
        {
            AccountUid = request.AccountUid,
            ProductCode = request.ProductCode,
            EstimateAmount = request.EstimateAmount,
            ReferenceKind = WalletTxnReferenceKind.Hold,
            Ttl = request.Ttl,
            IdempotencyKey = request.IdempotencyKey,
        });

        return new PlatformHoldResult(
            hold.Success,
            hold.HoldId,
            hold.AvailableAfter,
            hold.FailureCode,
            hold.FailureMessage);
    }

    public async Task<PlatformCommitHoldResult> CommitHoldAsync(long holdId, decimal actualAmount, string idempotencyKey)
    {
        var commit = await _billing.Metering.CommitHoldAsync(holdId, actualAmount, idempotencyKey);
        return new PlatformCommitHoldResult(commit.Success, commit.BillSerialNo, null, null);
    }

    public async Task<bool> ReleaseHoldAsync(long holdId, string reason)
        => await _billing.Metering.ReleaseHoldAsync(holdId, reason);

    public async Task<IReadOnlyDictionary<string, PlatformCommitBillLookup>> LookupCommitBillsAsync(
        IEnumerable<string> idempotencyKeys)
    {
        var keys = idempotencyKeys
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        if (keys.Length == 0)
            return new Dictionary<string, PlatformCommitBillLookup>(StringComparer.Ordinal);

        var lookup = await _billing.Metering.LookupCommitBillsByIdempotencyKeysAsync(keys);
        return lookup.Items.ToDictionary(
            i => i.IdempotencyKey,
            i => new PlatformCommitBillLookup(
                i.IdempotencyKey,
                i.BillId,
                i.Status,
                i.ReversedAtUtc,
                i.ReversedCode),
            StringComparer.Ordinal);
    }

    public async Task<PlatformReverseCommitResult> ReverseCommitAsync(PlatformReverseCommitRequest request)
    {
        var result = await _billing.Metering.ReverseCommitByIdempotencyKeyAsync(
            new ReverseCommitByKeyCommand
            {
                IdempotencyKey = request.IdempotencyKey,
                Code = request.Code,
                Note = request.Note,
                OperatorIamUserUid = request.OperatorIamUserUid,
            });

        return new PlatformReverseCommitResult(result.Success, result.FailureCode, result.FailureMessage);
    }

    public async Task<decimal?> GetWalletAvailableAsync(long accountUid)
    {
        var snap = await _billing.Wallet.GetWalletAsync(accountUid);
        return snap?.Available;
    }

    public async Task<decimal> GetProductUsageAmountAsync(
        long accountUid,
        string productCode,
        DateTime fromUtc,
        DateTime toUtc)
    {
        var summary = await _billing.Metering.GetProductUsageSummaryAsync(
            new ProductUsageSummaryQuery
            {
                AccountUid = accountUid,
                ProductCode = productCode,
                FromUtc = fromUtc,
                ToUtc = toUtc,
            });
        return summary?.Amount ?? 0m;
    }

    public async Task CreateInternalRechargeAsync(PlatformInternalRechargeRequest request)
    {
        await _billing.RechargeAdmin.CreateInternalAsync(new CreateInternalRechargeCommand
        {
            AccountUid = request.AccountUid,
            Amount = request.Amount,
            Source = request.Source,
            Note = request.Note,
            OperatorUid = request.OperatorUid,
            IdempotencyKey = request.IdempotencyKey,
            ProductCode = request.ProductCode,
        });
    }
}
