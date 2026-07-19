namespace Product.Common.Auth.BackendRpc.Credentials;

/// <summary>
/// 外部运行时回调 BackendRpc 时的凭据签发与验签。ClientSecret 自带 HMAC 签名，鉴权无需查库。
/// </summary>
public interface IBackendRpcCredentialSigner
{
    string IssueSecret(string clientId);
    bool VerifySecret(string clientId, string clientSecret);
}
