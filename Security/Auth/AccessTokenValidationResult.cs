namespace Product.Common.Auth;

public sealed record AccessTokenValidationResult(
    string? Jti,
    long AccountUid,
    long? IamUserUid,
    string? Actor,
    string? Role,
    long? StaffUid,
    string? AccountType);
