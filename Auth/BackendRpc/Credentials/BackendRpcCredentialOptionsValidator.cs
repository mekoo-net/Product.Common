using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Product.Common.Auth.BackendRpc.Options;

namespace Product.Common.Auth.BackendRpc.Credentials;

public sealed class BackendRpcCredentialOptionsValidator(
    IHostEnvironment env,
    string hmacKeyConfigPath) : IValidateOptions<BackendRpcCredentialOptions>
{
    public ValidateOptionsResult Validate(string? name, BackendRpcCredentialOptions options)
    {
        if (env.IsDevelopment() || string.Equals(env.EnvironmentName, "Testing", StringComparison.OrdinalIgnoreCase))
            return ValidateOptionsResult.Success;

        return BackendRpcCryptoValidation.ValidateBase64HmacKey(options.HmacKeyBase64, hmacKeyConfigPath);
    }
}

internal static class BackendRpcCryptoValidation
{
    public static ValidateOptionsResult ValidateBase64HmacKey(string? raw, string configPath)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return ValidateOptionsResult.Fail($"{configPath} is required in non-Development environments.");

        byte[] decoded;
        try { decoded = Convert.FromBase64String(raw); }
        catch (FormatException)
        {
            return ValidateOptionsResult.Fail($"{configPath} must be a valid Base64 string.");
        }

        return decoded.Length < 16
            ? ValidateOptionsResult.Fail($"{configPath} must decode to at least 16 bytes (got {decoded.Length}).")
            : ValidateOptionsResult.Success;
    }
}
