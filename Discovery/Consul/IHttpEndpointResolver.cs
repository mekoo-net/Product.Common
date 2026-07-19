namespace Product.Common.Discovery;

/// <summary>
/// 从 Consul 健康实例解析服务的对外 HTTP base URL（登录 bootstrap、客户端直连等）。
/// </summary>
public interface IHttpEndpointResolver
{
    Task<Uri> ResolvePublicUrlAsync(string serviceName, CancellationToken cancellationToken = default);
}
