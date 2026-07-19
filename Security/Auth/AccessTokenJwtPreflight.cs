using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Product.Common.Auth;

/// <summary>
/// 已解析 JWT 的廉价预检（不验签）。须在 <see cref="JsonWebTokenHandler.ReadJsonWebToken"/> 之后调用。
/// </summary>
public static class AccessTokenJwtPreflight
{
    public static string? DescribeFailure(JsonWebToken jwt, GatewayJwtOptions options)
    {
        if (!string.Equals(jwt.Alg, SecurityAlgorithms.EcdsaSha256, StringComparison.Ordinal))
            return $"unsupported alg '{jwt.Alg}'";

        var now = DateTime.UtcNow;
        var skew = options.ClockSkew;
        if (jwt.ValidTo <= now.Subtract(skew))
            return $"expired (exp={jwt.ValidTo:O}, now={now:O})";

        if (jwt.ValidFrom > now.Add(skew))
            return $"not yet valid (nbf={jwt.ValidFrom:O}, now={now:O})";

        if (string.IsNullOrEmpty(jwt.Issuer))
            return "missing issuer";

        var expectedAudience = ResolveExpectedAudience(jwt.Issuer, options);
        if (expectedAudience is null)
            return $"unknown issuer '{jwt.Issuer}'";

        if (!HasAudience(jwt, expectedAudience))
            return $"audience mismatch (expected={expectedAudience})";

        var actor = jwt.TryGetClaim("actor", out var actorClaim) ? actorClaim.Value : null;
        if (string.Equals(actor, "staff", StringComparison.OrdinalIgnoreCase))
        {
            var stuid = jwt.TryGetClaim("stuid", out var stuidClaim) ? stuidClaim.Value : null;
            return long.TryParse(stuid, out var staffUid) && staffUid > 0
                ? null
                : $"invalid staff token (stuid='{stuid ?? "(null)"}')";
        }

        var acct = jwt.TryGetClaim("acct", out var acctClaim) ? acctClaim.Value : null;
        return long.TryParse(acct, out var accountUid) && accountUid > 0
            ? null
            : $"invalid account token (acct='{acct ?? "(null)"}')";
    }

    private static string? ResolveExpectedAudience(string issuer, GatewayJwtOptions options)
    {
        if (string.Equals(issuer, options.Account.Issuer, StringComparison.Ordinal))
            return options.Account.Audience;
        if (string.Equals(issuer, options.Staff.Issuer, StringComparison.Ordinal))
            return options.Staff.Audience;
        return null;
    }

    private static bool HasAudience(JsonWebToken jwt, string expectedAudience)
    {
        foreach (var aud in jwt.Audiences)
        {
            if (string.Equals(aud, expectedAudience, StringComparison.Ordinal))
                return true;
        }

        foreach (var claim in jwt.Claims)
        {
            if (!string.Equals(claim.Type, "aud", StringComparison.OrdinalIgnoreCase))
                continue;
            if (string.Equals(claim.Value, expectedAudience, StringComparison.Ordinal))
                return true;
        }

        return false;
    }
}
