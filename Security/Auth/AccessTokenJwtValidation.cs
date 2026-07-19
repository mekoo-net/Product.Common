namespace Product.Common.Auth;

public readonly record struct AccessTokenJwtValidation(
    AccessTokenValidationResult? Result,
    string? FailureReason)
{
    public bool IsValid => Result is not null;
}
