using Meeko.Contracts.Storage;
using Product.Common.Discovery;

namespace Product.Common.PlatformServices;

/// <summary>Storage 服务组（Consul: <c>storage</c>）。Product 侧精简客户端。</summary>
public sealed class StoragePlatformService : PlatformServiceGroup
{
    public StoragePlatformService(IGrpcChannelFactory channels) : base(channels)
    {
        Sign = CreateClient<IStorageSignService>();
    }

    protected override string ConsulGroupName => "storage";

    public IStorageSignService Sign { get; }
}
