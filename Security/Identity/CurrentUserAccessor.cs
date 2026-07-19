using Microsoft.AspNetCore.Http;
using Product.Common.Discovery;

namespace Product.Common.Identity;

public sealed class CurrentUserAccessor : ICurrentUser
{
    public const string UserIdHeader     = GatewayHeaders.UserId;
    public const string IamUidHeader     = GatewayHeaders.IamUid;
    public const string RolesHeader      = GatewayHeaders.Roles;
    public const string TraceIdHeader    = GatewayHeaders.TraceId;
    public const string ActorHeader      = GatewayHeaders.Actor;
    public const string StaffRoleHeader  = GatewayHeaders.StaffRole;

    private readonly IHttpContextAccessor _accessor;

    public CurrentUserAccessor(IHttpContextAccessor accessor) => _accessor = accessor;

    private HttpContext? Ctx => _accessor.HttpContext;

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(UserId);

    public string? UserId     => Ctx?.Request.Headers[UserIdHeader].FirstOrDefault();
    public string? IamUserUid => Ctx?.Request.Headers[IamUidHeader].FirstOrDefault();
    public string? TraceId    => Ctx?.Request.Headers[TraceIdHeader].FirstOrDefault();
    public string? Actor     => Ctx?.Request.Headers[ActorHeader].FirstOrDefault();
    public string? StaffRole => Ctx?.Request.Headers[StaffRoleHeader].FirstOrDefault();

    public IReadOnlyCollection<string> Roles
    {
        get
        {
            var raw = Ctx?.Request.Headers[RolesHeader].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(raw))
            {
                return Array.Empty<string>();
            }

            return raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }
    }

    public bool IsInRole(string role)
        => Roles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
}
