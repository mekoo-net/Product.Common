using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Product.Common.Auth;

/// <summary>ES256 (P-256) JWT 密钥材料：签发、JWKS 导出、验签。</summary>
public static class JwtEcKeyMaterial
{
    public static ECDsa LoadPrivateKey(string privateKeyPem)
    {
        if (string.IsNullOrWhiteSpace(privateKeyPem))
            throw new InvalidOperationException("JWT private key PEM is empty.");

        var ecdsa = ECDsa.Create();
        ecdsa.ImportFromPem(privateKeyPem);
        return ecdsa;
    }

    public static ECDsaSecurityKey CreatePrivateSecurityKey(string privateKeyPem, string keyId)
    {
        var ecdsa = LoadPrivateKey(privateKeyPem);
        return new ECDsaSecurityKey(ecdsa) { KeyId = keyId };
    }

    public static SigningCredentials CreateSigningCredentials(string privateKeyPem, string keyId)
    {
        var key = CreatePrivateSecurityKey(privateKeyPem, keyId);
        return new SigningCredentials(key, SecurityAlgorithms.EcdsaSha256);
    }

    public static JsonWebKey ExportPublicJwk(string privateKeyPem, string keyId)
    {
        using var ecdsa = LoadPrivateKey(privateKeyPem);
        var parameters = ecdsa.ExportParameters(false);
        if (parameters.Q.X is null || parameters.Q.Y is null)
            throw new InvalidOperationException("EC public key coordinates are missing.");

        return new JsonWebKey
        {
            Kty = "EC",
            Crv = JsonWebKeyECTypes.P256,
            X = Base64UrlEncoder.Encode(parameters.Q.X),
            Y = Base64UrlEncoder.Encode(parameters.Q.Y),
            KeyId = keyId,
            Alg = SecurityAlgorithms.EcdsaSha256,
            Use = JsonWebKeyUseNames.Sig,
        };
    }

    public static IEnumerable<SecurityKey> KeysFromJwks(JsonWebKeySet set)
        => set.GetSigningKeys();

    public static SecurityKey KeyFromPublicJwk(
        string keyId,
        string kty,
        string crv,
        string x,
        string y,
        string alg,
        string use)
    {
        var jwk = new JsonWebKey
        {
            KeyId = keyId,
            Kty = kty,
            Crv = crv,
            X = x,
            Y = y,
            Alg = alg,
            Use = use,
        };
        return new JsonWebKeySet { Keys = { jwk } }.GetSigningKeys().First();
    }
}
