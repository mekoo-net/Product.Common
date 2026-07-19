using Meeko.Contracts.Keystone;
using Product.Common.PlatformServices;

namespace Product.Common.Keystone;

public sealed record PlatformAccountListQuery(
    int Page,
    int PageSize,
    string? ContactKeyword,
    string Status = "all",
    string Type = "all");

public sealed record PlatformAccountListItem(
    long Uid,
    string? DisplayName,
    string? Email,
    string? Status,
    DateTime CreatedAtUtc,
    DateTime? LastActiveAtUtc);

public sealed record PlatformAccountListResult(
    IReadOnlyList<PlatformAccountListItem> Items,
    int Total);

public sealed record PlatformAccountDetail(
    long Uid,
    string? DisplayName,
    string? Email,
    string? Status,
    DateTime CreatedAtUtc,
    DateTime? LastActiveAtUtc);

public sealed record PlatformAccountContact(
    long Uid,
    string? DisplayName,
    string? Email,
    string? Phone);

public sealed record PlatformAccountCommandResult(bool Success, string? FailureMessage);

public sealed class PlatformKeystoneAccountsFacade
{
    private readonly KeystonePlatformService _keystone;

    public PlatformKeystoneAccountsFacade(KeystonePlatformService keystone) => _keystone = keystone;

    public async Task<PlatformAccountListResult> ListAccountsAsync(PlatformAccountListQuery query)
    {
        var result = await _keystone.AccountAdmin.ListAccountsAsync(new ListAccountsQuery
        {
            Page = query.Page,
            PageSize = query.PageSize,
            ContactKeyword = query.ContactKeyword,
            Status = query.Status,
            Type = query.Type,
        });

        return new PlatformAccountListResult(
            result.Items.Select(MapListItem).ToArray(),
            result.Total);
    }

    public async Task<PlatformAccountDetail?> GetAccountAsync(long accountUid)
    {
        var detail = await _keystone.AccountAdmin.GetAccountAdminAsync(accountUid);
        return detail is null ? null : MapDetail(detail);
    }

    public async Task<IReadOnlyDictionary<long, PlatformAccountContact>> GetAccountContactsAsync(
        IReadOnlyCollection<long> accountUids)
    {
        if (accountUids.Count == 0)
            return new Dictionary<long, PlatformAccountContact>();

        var batch = await _keystone.AccountAdmin.GetAccountContactsAsync(accountUids.ToArray());
        return batch.Items.ToDictionary(
            c => c.Uid,
            c => new PlatformAccountContact(c.Uid, c.DisplayName, c.Email, c.Phone));
    }

    public async Task<PlatformAccountCommandResult> UpdateAccountAsync(long accountUid, string? displayName)
    {
        var result = await _keystone.AccountAdmin.UpdateAccountAsync(new UpdateAccountCommand
        {
            AccountUid = accountUid,
            DisplayName = displayName,
        });
        return new PlatformAccountCommandResult(result.Success, result.FailureMessage);
    }

    public async Task<PlatformAccountCommandResult> SetAccountStatusAsync(long accountUid, string status)
    {
        var result = await _keystone.AccountAdmin.SetAccountStatusAsync(new SetAccountStatusCommand
        {
            AccountUid = accountUid,
            Status = status,
        });
        return new PlatformAccountCommandResult(result.Success, result.FailureMessage);
    }

    private static PlatformAccountListItem MapListItem(AccountAdminListItem item) =>
        new(
            item.Uid,
            item.DisplayName ?? item.Owner?.DisplayName,
            item.Owner?.Email,
            item.Status,
            item.CreatedAtUtc,
            item.Active?.LastActiveAtUtc);

    private static PlatformAccountDetail MapDetail(AccountAdminDetail detail) =>
        new(
            detail.Uid,
            detail.DisplayName ?? detail.Owner?.DisplayName,
            detail.Owner?.Email,
            detail.Status,
            detail.CreatedAtUtc,
            detail.Active?.LastActiveAtUtc);
}
