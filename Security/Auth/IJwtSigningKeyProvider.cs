using Microsoft.IdentityModel.Tokens;

namespace Product.Common.Auth;

/// <summary>ES256 验签公钥来源（HTTP JWKS 或 Keystone RPC）。</summary>
public interface IJwtSigningKeyProvider
{
    IReadOnlyCollection<SecurityKey> SigningKeys { get; }

    IEnumerable<SecurityKey> ResolveSigningKeys(string? kid);
}
