using Meeko.Contracts.Gateway.Auth;
using Meeko.Contracts.Keystone;
using Product.Common.Discovery;

namespace Product.Common.PlatformServices;

/// <summary>Keystone 服务组（Consul: <c>keystone</c>）。Product 侧精简客户端。</summary>
public sealed class KeystonePlatformService : PlatformServiceGroup
{
    public KeystonePlatformService(IGrpcChannelFactory channels) : base(channels)
    {
        Query = CreateClient<IKeystoneQueryService>();
        AccountAdmin = CreateClient<IKeystoneAccountAdminService>();
        StaffAdmin = CreateClient<IKeystoneStaffAdminService>();
        GatewayAuth = CreateClient<IGatewayAuthService>();
    }

    protected override string ConsulGroupName => "keystone";

    public IKeystoneQueryService Query { get; }
    public IKeystoneAccountAdminService AccountAdmin { get; }
    public IKeystoneStaffAdminService StaffAdmin { get; }
    public IGatewayAuthService GatewayAuth { get; }
}
