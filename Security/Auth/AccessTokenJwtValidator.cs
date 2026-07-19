using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Product.Common.Auth;

/// <summary>网关侧 ES256 access JWT：一次解析 → 预检 → 验签。</summary>
public sealed class AccessTokenJwtValidator
{
    private const int MaxTokenLength = 8192;

    private readonly GatewayJwtOptions _options;
    private readonly IJwtSigningKeyProvider _jwks;
    private readonly JsonWebTokenHandler _handler = new()
    {
        MapInboundClaims = false,
    };

    public AccessTokenJwtValidator(GatewayJwtOptions options, IJwtSigningKeyProvider jwks)
    {
        _options = options;
        _jwks = jwks;
    }

    public AccessTokenJwtValidation Validate(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            return Fail("empty token");

        if (accessToken.Length > MaxTokenLength)
            return Fail($"token too long ({accessToken.Length} > {MaxTokenLength})");

        JsonWebToken jwt;
        try
        {
            jwt = _handler.ReadJsonWebToken(accessToken);
        }
        catch (Exception ex)
        {
            return Fail($"unreadable jwt: {ex.Message}");
        }

        var preflightFailure = AccessTokenJwtPreflight.DescribeFailure(jwt, _options);
        if (preflightFailure is not null)
            return Fail(preflightFailure);

        TokenValidationResult validation;
        try
        {
            validation = _handler.ValidateTokenAsync(jwt, BuildParameters()).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            return Fail($"signature validation failed: {ex.Message}");
        }

        if (!validation.IsValid)
        {
            return Fail(validation.Exception?.Message ?? "invalid token signature");
        }

        return new AccessTokenJwtValidation(BuildResult(jwt), null);
    }

    private static AccessTokenJwtValidation Fail(string reason)
        => new(null, reason);

    private static AccessTokenValidationResult BuildResult(JsonWebToken jwt)
    {
        var actor = jwt.TryGetClaim("actor", out var actorClaim) ? actorClaim.Value : null;
        long accountUid = 0;
        long? iamUserUid = null;
        long? staffUid = null;

        if (string.Equals(actor, "staff", StringComparison.OrdinalIgnoreCase))
        {
            var stuid = jwt.TryGetClaim("stuid", out var stuidClaim) ? stuidClaim.Value : null;
            _ = long.TryParse(stuid, out var parsedStaff);
            staffUid = parsedStaff;
        }
        else
        {
            var acct = jwt.TryGetClaim("acct", out var acctClaim) ? acctClaim.Value : null;
            _ = long.TryParse(acct, out accountUid);
            var iuid = jwt.TryGetClaim("iuid", out var iuidClaim) ? iuidClaim.Value : null;
            if (long.TryParse(iuid, out var parsedIam) && parsedIam > 0)
                iamUserUid = parsedIam;
        }

        var role = jwt.TryGetClaim("role", out var roleClaim) ? roleClaim.Value : null;
        var atype = jwt.TryGetClaim("atype", out var atypeClaim) ? atypeClaim.Value : null;

        return new AccessTokenValidationResult(
            Jti: jwt.Id,
            AccountUid: accountUid,
            IamUserUid: iamUserUid,
            Actor: actor,
            Role: role,
            StaffUid: staffUid,
            AccountType: atype);
    }

    private TokenValidationParameters BuildParameters()
        => _options.BuildTokenValidationParameters(_jwks);
}
