using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Product.Common.Auth;

/// <summary>
/// Gateway JWT 验签配置（ES256）。私钥仅在 Keystone；公钥经 RPC 或 JWKS URI 拉取。
/// </summary>
public sealed class GatewayJwtOptions
{
    public const string SectionName = "Jwt";

    /// <summary>Hosted gateway HTTP JWKS（Demux/Tavern）。Meeko.Gateway 走 Keystone RPC，无需此项。</summary>
    public string JwksUri { get; set; } = "";

    public AccountJwtSection Account { get; set; } = new();
    public StaffJwtSection Staff { get; set; } = new();
    public TimeSpan ClockSkew { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan JwksRefreshInterval { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// account/staff 双 issuer 共用的验签参数。唯一实现，供本地校验（<see cref="AccessTokenJwtValidator"/>）
    /// 和 ASP.NET JwtBearer（<see cref="GatewayJwtExtensions"/>）共用，避免两处各写一份。
    /// </summary>
    public TokenValidationParameters BuildTokenValidationParameters(IJwtSigningKeyProvider jwks)
    {
        var validIssuers = DistinctNonEmpty(Account.Issuer, Staff.Issuer);
        var validAudiences = DistinctNonEmpty(Account.Audience, Staff.Audience);

        return new TokenValidationParameters
        {
            ValidateIssuer = validIssuers.Length > 0,
            ValidateAudience = validAudiences.Length > 0,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuers = validIssuers,
            ValidAudiences = validAudiences,
            ClockSkew = ClockSkew,
            IssuerSigningKeyResolver = (_, securityToken, kid, _) =>
            {
                var resolvedKid = securityToken is JsonWebToken jwt && !string.IsNullOrEmpty(jwt.Kid)
                    ? jwt.Kid
                    : kid;
                return jwks.ResolveSigningKeys(resolvedKid).ToList();
            },
        };
    }

    private static string[] DistinctNonEmpty(params string[] values)
        => values.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct(StringComparer.Ordinal).ToArray();

    public sealed class AccountJwtSection
    {
        public const string DefaultIssuer = "https://keystone.meeko.local/account";
        public const string DefaultAudience = "meeko-api";

        public string Issuer { get; set; } = DefaultIssuer;
        public string Audience { get; set; } = DefaultAudience;
    }

    public sealed class StaffJwtSection
    {
        public const string DefaultIssuer = "https://keystone.meeko.local/staff";
        public const string DefaultAudience = "meeko-admin";

        public string Issuer { get; set; } = DefaultIssuer;
        public string Audience { get; set; } = DefaultAudience;
    }
}
