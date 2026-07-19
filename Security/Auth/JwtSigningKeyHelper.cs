using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Product.Common.Auth;

internal static class JwtSigningKeyHelper
{
    /// <summary>HS256 要求至少 128 bits（16 bytes）。</summary>
    private const int MinKeySizeBytes = 16;

    public static SymmetricSecurityKey Create(string signingKey, string configPath)
    {
        if (string.IsNullOrWhiteSpace(signingKey))
        {
            throw new InvalidOperationException(
                $"{configPath} is empty. Configure it in this service's /config/<ServiceName>.yaml file.");
        }

        var keyBytes = Encoding.UTF8.GetBytes(signingKey);
        if (keyBytes.Length >= MinKeySizeBytes)
        {
            return new SymmetricSecurityKey(keyBytes);
        }

        var hint = string.Equals(signingKey, "__SECRET__", StringComparison.Ordinal)
            ? " Replace the '__SECRET__' placeholder with a key at least 16 characters long."
            : string.Empty;

        throw new InvalidOperationException(
            $"{configPath} is {keyBytes.Length} bytes ({keyBytes.Length * 8} bits); " +
            $"HS256 requires at least {MinKeySizeBytes} bytes (128 bits).{hint} " +
            "Gateway.yaml and Keystone.yaml must each define Jwt:Account:SigningKey and Jwt:Staff:SigningKey with the same values.");
    }
}
