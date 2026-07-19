using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Product.Common.Auth.BackendRpc.Options;

namespace Product.Common.Auth.BackendRpc.Credentials;

/// <summary>
/// 无状态 BackendRpc 凭据。格式：<c>cs-{nonce}.{signature}</c>，
/// signature = HMACSHA256(HmacKey, "{clientId}.{nonce}")。
/// </summary>
public sealed class HmacBackendRpcCredentialSigner : IBackendRpcCredentialSigner
{
    private readonly byte[] _key;
    private readonly int _secretRandomBytes;

    public HmacBackendRpcCredentialSigner(IOptions<BackendRpcCredentialOptions> options)
    {
        var raw = options.Value.HmacKeyBase64;
        if (string.IsNullOrWhiteSpace(raw))
            throw new InvalidOperationException("BackendRpc HmacKeyBase64 is required.");

        _key = Convert.FromBase64String(raw);
        if (_key.Length < 16)
            throw new InvalidOperationException("BackendRpc HmacKeyBase64 must decode to at least 16 bytes.");

        _secretRandomBytes = options.Value.SecretRandomBytes <= 0 ? 32 : options.Value.SecretRandomBytes;
    }

    public string IssueSecret(string clientId)
    {
        if (string.IsNullOrWhiteSpace(clientId))
            throw new ArgumentException("clientId required", nameof(clientId));

        var nonce = Base64UrlEncode(RandomNumberGenerator.GetBytes(_secretRandomBytes));
        var signature = Sign(clientId, nonce);
        return $"cs-{nonce}.{signature}";
    }

    public bool VerifySecret(string clientId, string clientSecret)
    {
        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
            return false;
        if (!clientSecret.StartsWith("cs-", StringComparison.Ordinal))
            return false;

        var body = clientSecret[3..];
        var separator = body.IndexOf('.');
        if (separator <= 0 || separator == body.Length - 1)
            return false;

        var nonce = body[..separator];
        var signature = body[(separator + 1)..];
        var expected = Encoding.ASCII.GetBytes(Sign(clientId, nonce));
        var actual = Encoding.ASCII.GetBytes(signature);
        return expected.Length == actual.Length
               && CryptographicOperations.FixedTimeEquals(expected, actual);
    }

    private string Sign(string clientId, string nonce)
    {
        var payload = Encoding.UTF8.GetBytes($"{clientId}.{nonce}");
        return Base64UrlEncode(HMACSHA256.HashData(_key, payload));
    }

    private static string Base64UrlEncode(byte[] data)
        => Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
