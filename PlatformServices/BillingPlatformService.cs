using Meeko.Contracts.Billing;
using Product.Common.Discovery;
using Platform.Common.Discovery;

namespace Product.Common.PlatformServices;

/// <summary>Billing 服务组（Consul: <c>billing</c>）。Product 侧精简客户端。</summary>
public sealed class BillingPlatformService : PlatformServiceGroup
{
    public BillingPlatformService(IGrpcChannelFactory channels) : base(channels)
    {
        Metering = CreateClient<IBillingMeteringService>();
        Wallet = CreateClient<IBillingWalletService>();
        RechargeAdmin = CreateClient<IBillingRechargeAdminService>();
    }

    protected override string ConsulGroupName => ServiceKeys.Billing;

    public IBillingMeteringService Metering { get; }
    public IBillingWalletService Wallet { get; }
    public IBillingRechargeAdminService RechargeAdmin { get; }
}
