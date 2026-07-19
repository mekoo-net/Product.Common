using MessagePack;

namespace Meeko.Contracts.Billing;

[MessagePackObject]
public sealed class ReverseBillResult
{
    [Key(0)] public required bool Success { get; set; }
    [Key(1)] public BillDto? Bill { get; set; }
    [Key(2)] public string? FailureCode { get; set; }
    [Key(3)] public string? FailureMessage { get; set; }

    public static ReverseBillResult Ok(BillDto bill) =>
        new() { Success = true, Bill = bill };

    public static ReverseBillResult Fail(string code, string message) =>
        new() { Success = false, FailureCode = code, FailureMessage = message };
}
