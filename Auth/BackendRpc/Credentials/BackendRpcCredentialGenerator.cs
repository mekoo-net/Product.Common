using System.Security.Cryptography;

namespace Product.Common.Auth.BackendRpc.Credentials;

public sealed class BackendRpcCredentialGenerator
{
    public string GenerateClientId()
    {
        var bytes = RandomNumberGenerator.GetBytes(16);
        return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}
