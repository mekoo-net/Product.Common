namespace Product.Common.Auth.BackendRpc.Options;

public sealed class BackendRpcCredentialOptions
{
    public string? HmacKeyBase64 { get; set; }
    public int SecretRandomBytes { get; set; } = 32;
}
