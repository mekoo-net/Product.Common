using Grpc.Core;
using MagicOnion.Client;
using Microsoft.AspNetCore.Http;

namespace Product.Common.Discovery;

/// <summary>
/// MagicOnion 客户端过滤器：每次 RPC 从当前 HTTP 上下文（或 Activity/Correlation 兜底）透传身份与关联 ID。
/// </summary>
public sealed class OutgoingContextPropagationFilter : IClientFilter
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OutgoingContextPropagationFilter(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    public async ValueTask<ResponseContext> SendAsync(
        RequestContext context,
        Func<RequestContext, ValueTask<ResponseContext>> next)
    {
        var metadata = OutgoingContextMetadata.Build(_httpContextAccessor);
        if (metadata.Count > 0)
        {
            var headers = context.CallOptions.Headers;
            if (headers is not null)
            {
                foreach (var entry in metadata)
                {
                    if (!headers.Any(e => string.Equals(e.Key, entry.Key, StringComparison.OrdinalIgnoreCase)))
                    {
                        headers.Add(entry);
                    }
                }
            }
        }

        return await next(context);
    }
}
