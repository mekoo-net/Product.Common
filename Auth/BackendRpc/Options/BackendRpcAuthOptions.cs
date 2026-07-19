namespace Product.Common.Auth.BackendRpc.Options;

/// <summary>
/// BackendRpc HMAC 鉴权中间件配置（header 名与错误文案由产品自行定义）。
/// </summary>
public sealed class BackendRpcAuthOptions
{
    public string ClientIdHeader { get; set; } = "";
    public string ClientSecretHeader { get; set; } = "";
    public string MissingCredentialsMessage { get; set; } = "";
    public string InvalidCredentialsMessage { get; set; } = "";
}
