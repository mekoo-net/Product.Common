using Grpc.Net.Client;
using MagicOnion;
using Product.Common.Discovery;

namespace Product.Common.PlatformServices;

/// <summary>
/// 平台 Consul 服务组 RPC 基类。子类声明组名并在构造时创建 MagicOnion 客户端。
/// </summary>
public abstract class PlatformServiceGroup : IAsyncDisposable
{
    private readonly GrpcChannel _channel;

    protected PlatformServiceGroup(IGrpcChannelFactory channels)
    {
        _channel = channels.CreateChannel(ConsulGroupName);
        Channels = channels;
    }

    private IGrpcChannelFactory Channels { get; }

    /// <summary>Consul 服务 Key，取自 <see cref="Platform.Common.Discovery.ServiceKeys"/>，与对端注册时用的是同一个常量。</summary>
    protected abstract string ConsulGroupName { get; }

    protected T CreateClient<T>() where T : IService<T> => Channels.CreateClient<T>(_channel);

    public async ValueTask DisposeAsync() => await _channel.ShutdownAsync();
}
