using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Product.Common.Auth.BackendRpc.Credentials;
using Product.Common.Auth.BackendRpc.Options;

namespace Product.Common.Auth.BackendRpc.Middleware;

/// <summary>
/// BackendRpc MagicOnion 面的 HMAC clientId/clientSecret 鉴权。
/// </summary>
public sealed class BackendRpcAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly BackendRpcAuthOptions _options;

    public BackendRpcAuthMiddleware(RequestDelegate next, IOptions<BackendRpcAuthOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context, IBackendRpcCredentialSigner credentialSigner)
    {
        var clientId = context.Request.Headers[_options.ClientIdHeader].ToString();
        var clientSecret = context.Request.Headers[_options.ClientSecretHeader].ToString();
        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
        {
            await WriteGrpcErrorAsync(context, StatusCode.Unauthenticated, _options.MissingCredentialsMessage);
            return;
        }

        if (!credentialSigner.VerifySecret(clientId, clientSecret))
        {
            await WriteGrpcErrorAsync(context, StatusCode.Unauthenticated, _options.InvalidCredentialsMessage);
            return;
        }

        await _next(context);
    }

    private static async Task WriteGrpcErrorAsync(HttpContext context, StatusCode statusCode, string message)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "application/grpc";
        context.Response.AppendTrailer("grpc-status", ((int)statusCode).ToString());
        context.Response.AppendTrailer("grpc-message", Uri.EscapeDataString(message));
        await context.Response.CompleteAsync();
    }
}
