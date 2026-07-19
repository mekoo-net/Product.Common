using System.IdentityModel.Tokens.Jwt;

namespace Product.Common.Auth;

public static class AccessTokenCache
{
    public static TimeSpan PositiveTtl(string accessToken, int redisTtlSeconds, int maxTtlSeconds)
    {
        var ttl = TimeSpan.FromSeconds(Math.Clamp(redisTtlSeconds, 1, Math.Max(1, maxTtlSeconds)));
        var remaining = TryGetJwtRemaining(accessToken);
        if (remaining is { } r && r > TimeSpan.Zero && r < ttl)
            return r;

        return ttl;
    }

    public static string KeyHash(string accessToken)
        => Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(accessToken)));

    private static TimeSpan? TryGetJwtRemaining(string accessToken)
    {
        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var remaining = jwt.ValidTo - DateTime.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
        catch
        {
            return null;
        }
    }
}
