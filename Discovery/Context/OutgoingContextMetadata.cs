using System.Diagnostics;
using Grpc.Core;
using Platform.Common.Correlation;
using Platform.Common.Discovery;
using Microsoft.AspNetCore.Http;

namespace Product.Common.Discovery;

/// <summary>
/// 构建出站 MagicOnion/gRPC metadata：透传 Gateway 注入的身份头、correlation_id 与 trace_id。
/// </summary>
public static class OutgoingContextMetadata
{
    private static readonly string[] PropagatedHeaders =
    [
        GatewayHeaders.UserId,
        GatewayHeaders.Roles,
        GatewayHeaders.TraceId,
        GatewayHeaders.Actor,
        GatewayHeaders.StaffRole,
        GatewayHeaders.AccountUid,
        GatewayHeaders.AccountType,
        GatewayHeaders.Role,
        GatewayHeaders.Scopes,
        GatewayHeaders.KeyUid,
        GatewayHeaders.StaffUid,
        GatewayHeaders.IamUid,
        GatewayHeaders.TokenJti,
        GatewayHeaders.SessionId,
        GatewayHeaders.ClientIp,
        CorrelationContext.HeaderName,
    ];

    public static Metadata Build(IHttpContextAccessor httpContextAccessor)
    {
        var md = new Metadata();
        var headers = httpContextAccessor.HttpContext?.Request.Headers;

        if (headers is not null)
        {
            foreach (var name in PropagatedHeaders)
            {
                var value = headers[name].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    md.Add(name.ToLowerInvariant(), value);
                }
            }
        }

        if (!ContainsKey(md, GatewayHeaders.TraceId))
        {
            var traceId = Activity.Current?.TraceId.ToString();
            if (!string.IsNullOrWhiteSpace(traceId))
            {
                md.Add(GatewayHeaders.TraceId.ToLowerInvariant(), traceId);
            }
        }

        if (!ContainsKey(md, CorrelationContext.HeaderName))
        {
            var correlationId = CorrelationContext.Current;
            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                md.Add(CorrelationContext.HeaderName.ToLowerInvariant(), correlationId);
            }
        }

        return md;
    }

    private static bool ContainsKey(Metadata metadata, string headerName)
        => metadata.Any(entry => string.Equals(entry.Key, headerName, StringComparison.OrdinalIgnoreCase));
}
