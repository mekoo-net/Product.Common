namespace Product.Common.Auth.BackendRpc.Credentials;

public sealed class BackendRpcCredentialIssuer(
    BackendRpcCredentialGenerator generator,
    IBackendRpcCredentialSigner signer)
{
    public IssuedBackendRpcCredentials Issue()
    {
        var clientId = generator.GenerateClientId();
        return new IssuedBackendRpcCredentials(clientId, signer.IssueSecret(clientId));
    }
}

public sealed record IssuedBackendRpcCredentials(string ClientId, string ClientSecret);
