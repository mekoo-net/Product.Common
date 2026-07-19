using MessagePack;

namespace Meeko.Contracts.Keystone;

[MessagePackObject]
public sealed class AccountAdminCommandResult
{
    [Key(0)] public required bool Success { get; set; }
    [Key(1)] public AccountAdminDetail? Account { get; set; }
    [Key(2)] public string? FailureCode { get; set; }
    [Key(3)] public string? FailureMessage { get; set; }

    public static AccountAdminCommandResult Ok(AccountAdminDetail detail) =>
        new() { Success = true, Account = detail };

    public static AccountAdminCommandResult Fail(string code, string message) =>
        new() { Success = false, FailureCode = code, FailureMessage = message };
}
