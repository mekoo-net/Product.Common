using System.Security.Claims;
using System.Text.Encodings.Web;
using Product.Common.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Product.Common.Auth;

/// <summary>
/// 下游服务（Bff 等）信任 Gateway 注入的身份 header，不再二次验 JWT。
/// </summary>
public static class GatewayForwardedAuthDefaults
{
    public const string AuthenticationScheme = "GatewayForwarded";
}

public static class GatewayForwardedAuthExtensions
{
    /// <summary>
    /// 从 Gateway 转发的 <c>X-User-Id</c> / <c>X-Actor</c> 等 header 构建 <see cref="ClaimsPrincipal"/>，
    /// 供 <c>[Authorize]</c> 使用；业务身份仍通过 <see cref="ICurrentUser"/> 读取同一组 header。
    /// </summary>
    public static IServiceCollection AddGatewayForwardedAuth(this IServiceCollection services)
    {
        services.AddAuthentication(GatewayForwardedAuthDefaults.AuthenticationScheme)
            .AddScheme<AuthenticationSchemeOptions, GatewayForwardedAuthenticationHandler>(
                GatewayForwardedAuthDefaults.AuthenticationScheme,
                _ => { });

        services.AddAuthorization();
        return services;
    }
}

internal sealed class GatewayForwardedAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public GatewayForwardedAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var userId = Request.Headers[CurrentUserAccessor.UserIdHeader].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new("sub", userId),
        };

        var actor = Request.Headers[CurrentUserAccessor.ActorHeader].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(actor))
        {
            claims.Add(new Claim("actor", actor));
        }

        var staffRole = Request.Headers[CurrentUserAccessor.StaffRoleHeader].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(staffRole))
        {
            claims.Add(new Claim("role", staffRole));
        }

        foreach (var role in Request.Headers[CurrentUserAccessor.RolesHeader].FirstOrDefault()
                     ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                 ?? [])
        {
            claims.Add(new Claim("role", role));
        }

        var staffUid = Request.Headers[CurrentActorAccessor.StaffUidHeader].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(staffUid))
        {
            claims.Add(new Claim("stuid", staffUid));
        }

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
