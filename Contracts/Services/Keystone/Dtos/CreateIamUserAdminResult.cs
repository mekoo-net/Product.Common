using MessagePack;

namespace Meeko.Contracts.Keystone;

[MessagePackObject]
public sealed class CreateIamUserAdminResult
{
    [Key(0)] public required bool Success { get; set; }
    [Key(1)] public IamUserInfo? IamUser { get; set; }
    [Key(2)] public string? FailureCode { get; set; }
    [Key(3)] public string? FailureMessage { get; set; }

    public static CreateIamUserAdminResult Ok(IamUserInfo iamUser) =>
        new() { Success = true, IamUser = iamUser };

    public static CreateIamUserAdminResult Fail(string code, string message) =>
        new() { Success = false, FailureCode = code, FailureMessage = message };
}
