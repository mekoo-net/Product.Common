using Consul;
using Grpc.Core;
using GrpcStatus = Grpc.Core.Status;
using Grpc.Net.Client;
using Grpc.Net.Client.Balancer;
using MagicOnion;
using MagicOnion.Client;
using Microsoft.Extensions.Logging;
using Platform.Common.Discovery;

namespace Product.Common.Discovery;

/// <summary>
/// gRPC 名称解析器：把 <c>consul:///&lt;service&gt;</c> 地址解析为该服务在 Consul 中所有健康实例的 gRPC 端点。
/// 解析完全异步，发生在首次 RPC（建立连接）时，而非进程启动期，因此不会造成服务间启动死锁。
/// 连接失败时 gRPC 会触发 <see cref="PollingResolver.Refresh"/> 自动重解析；实例增减则由后台
/// blocking query（X-Consul-Index 长轮询）感知——只有 index 变化才返回并发布，避免固定周期空转打爆 Consul 日志。
/// <para><b>禁止轮询</b>：除进程生命周期内一次冷启动 seed 外，对 Consul 的请求一律走 blocking query；
/// watch 断连时只退避后重挂 blocking query，不做定时/周期性非阻塞查询。</para>
/// </summary>
internal sealed class ConsulGrpcResolver : PollingResolver
{
    private static readonly TimeSpan WaitTime = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan MaxBackoff = TimeSpan.FromSeconds(30);

    private readonly string _serviceName;
    private readonly IConsulClient _consul;
    private readonly ILogger _logger;
    private readonly CancellationTokenSource _watchCts = new();
    private int _watchStarted;
    private int _seeded;
    private ulong _lastIndex;
    private volatile ServiceEntry[]? _snapshot;
    private List<BalancerAddress> _lastGoodAddresses = [];
    private int _degraded;

    public ConsulGrpcResolver(Uri address, IConsulClient consul, ILoggerFactory loggerFactory)
        : base(loggerFactory)
    {
        _serviceName = ParseServiceName(address);
        _consul = consul;
        _logger = loggerFactory.CreateLogger<ConsulGrpcResolver>();
    }

    // 发布（Listener）只在本方法内发生——gRPC 通过 Start()/Refresh() 在自己的解析周期里调用它。
    // 后台 watch loop 监听到变化时改用 Refresh() 触发本方法，而非从外部线程直接 Listener()，
    // 以避开 grpc-dotnet 的 Picker 中毒问题（issue #2407），并复用 LB 的解析锁。
    protected override async Task ResolveAsync(CancellationToken cancellationToken)
    {
        if (Interlocked.Exchange(ref _seeded, 1) == 0)
        {
            // 首次解析：同步做一次非阻塞查询，给通道快速喂端点，随后启动后台监听。
            try
            {
                var result = await _consul.Health.Service(
                    _serviceName, string.Empty, passingOnly: true, cancellationToken);
                _lastIndex = result.LastIndex;
                _snapshot = result.Response;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "Initial Consul resolve for '{Service}' failed", _serviceName);
            }
            finally
            {
                StartWatchLoop();
            }
        }

        PublishSnapshot();
    }

    private void StartWatchLoop()
    {
        // ResolveAsync 只在 Start() 之后被调用，此处启动后台 blocking query 监听是安全的。
        if (Interlocked.Exchange(ref _watchStarted, 1) == 0)
        {
            _ = Task.Run(() => WatchLoopAsync(_watchCts.Token));
        }
    }

    private async Task WatchLoopAsync(CancellationToken cancellationToken)
    {
        var backoff = TimeSpan.FromSeconds(1);
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var queryOptions = new QueryOptions
                {
                    WaitIndex = _lastIndex,
                    WaitTime = WaitTime,
                };

                var result = await _consul.Health.Service(
                    _serviceName, string.Empty, passingOnly: true, queryOptions, cancellationToken);

                // HashiCorp blocking-query 规范：index < 1 视为不可信，归一到 1，
                // 避免下一轮 WaitIndex=0 退化成非阻塞忙轮询打爆 Consul。
                var newIndex = result.LastIndex < 1 ? 1 : result.LastIndex;

                if (newIndex < _lastIndex)
                {
                    // index 回退视为 Consul 重置，下一轮用 0 重新拉全量。
                    _lastIndex = 0;
                }
                else if (newIndex != _lastIndex)
                {
                    // 有实例增减：更新快照并通过 gRPC 协调入口 Refresh() 触发 ResolveAsync 重新发布，
                    // 服务重新上线时即由此自动重新解析。
                    _lastIndex = newIndex;
                    _snapshot = result.Response;
                    Refresh();
                }
                // index 相等 == WaitTime 内无变化，blocking query 超时返回，继续监听。

                backoff = TimeSpan.FromSeconds(1);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex, "Consul watch for '{Service}' failed; retrying in {Backoff}s", _serviceName, backoff.TotalSeconds);

                try
                {
                    await Task.Delay(backoff, cancellationToken);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (backoff < MaxBackoff)
                    backoff = TimeSpan.FromSeconds(Math.Min(backoff.TotalSeconds * 2, MaxBackoff.TotalSeconds));
            }
        }
    }

    // 只在 ResolveAsync（gRPC 解析周期）内调用：把最新快照发布给通道。
    // 关键：瞬时查不到健康实例时保留 last-known-good，不向通道推 ForFailure 拆连接——
    // 让 gRPC 自己用连接重试 + RoundRobin 平滑实例抖动；真正的实例增减由 watch loop→Refresh 驱动。
    private void PublishSnapshot()
    {
        var entries = _snapshot ?? [];
        var addresses = new List<BalancerAddress>(entries.Length);
        var endpointUrls = new List<string>(entries.Length);
        foreach (var entry in entries)
        {
            if (ConsulGrpcAddress.TryResolve(entry, out var uri))
            {
                addresses.Add(new BalancerAddress(uri.Host, uri.Port));
                endpointUrls.Add(uri.GetLeftPart(UriPartial.Authority));
            }
        }

        if (addresses.Count > 0)
        {
            var wasDegraded = Interlocked.Exchange(ref _degraded, 0) == 1;
            _lastGoodAddresses = addresses;
            if (wasDegraded)
            {
                _logger.LogInformation(
                    "Consul service '{Service}' is healthy again; resolved {Count} gRPC endpoint(s): {Endpoints}",
                    _serviceName, addresses.Count, string.Join(", ", endpointUrls));
                // host:port 未变时 grpc 可能复用卡死的 subchannel；先 ForFailure 再 ForResult 强制重建。
                Listener(ResolverResult.ForFailure(new GrpcStatus(
                    StatusCode.Unavailable,
                    $"Consul service '{_serviceName}' recovered; resetting gRPC subchannels.")));
            }
            else
            {
                _logger.LogDebug(
                    "Resolved {Count} gRPC endpoint(s) for '{Service}' via Consul: {Endpoints}",
                    addresses.Count, _serviceName, string.Join(", ", endpointUrls));
            }

            Listener(ResolverResult.ForResult(addresses));
            return;
        }

        if (_lastGoodAddresses.Count > 0)
        {
            // 暂时无健康实例：维持上次可用端点，靠 gRPC 连接重试自动恢复（同地址重连）；
            // 若实例换了地址，watch loop 会在其重新注册时再次 Refresh 发布新端点。
            Interlocked.Exchange(ref _degraded, 1);
            _logger.LogWarning(
                "No healthy Consul instances for '{Service}' right now; keeping {Count} last-known-good endpoint(s)",
                _serviceName, _lastGoodAddresses.Count);
            Listener(ResolverResult.ForResult(_lastGoodAddresses));
            return;
        }

        Listener(ResolverResult.ForFailure(new GrpcStatus(
            StatusCode.Unavailable,
            $"No healthy Consul service instances found for '{_serviceName}'.")));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _watchCts.Cancel();
            _watchCts.Dispose();
        }

        base.Dispose(disposing);
    }

    private static string ParseServiceName(Uri address)
    {
        // consul:///billing -> Host 为空，服务名在 AbsolutePath；consul://billing -> 服务名在 Host。
        if (!string.IsNullOrEmpty(address.Host))
        {
            return address.Host;
        }

        return address.AbsolutePath.Trim('/');
    }
}

/// <summary>注册 <c>consul</c> scheme 的 gRPC 名称解析工厂。</summary>
internal sealed class ConsulGrpcResolverFactory : ResolverFactory
{
    private readonly IConsulClient _consul;

    public ConsulGrpcResolverFactory(IConsulClient consul)
    {
        _consul = consul;
    }

    public override string Name => "consul";

    public override Resolver Create(ResolverOptions options)
        => new ConsulGrpcResolver(options.Address, _consul, options.LoggerFactory);
}

/// <summary>
/// 创建面向某个 Consul 服务的 MagicOnion/gRPC 通道。通道创建本身是同步、廉价、且不接触 Consul 的；
/// 真实端点解析交给 <see cref="ConsulGrpcResolver"/> 在首次 RPC 时异步完成，并支持多实例轮询负载均衡。
/// </summary>
public interface IGrpcChannelFactory
{
    GrpcChannel CreateChannel(string serviceName);

    T CreateClient<T>(string serviceName) where T : IService<T>;

    T CreateClient<T>(GrpcChannel channel) where T : IService<T>;
}

internal sealed class GrpcChannelFactory : IGrpcChannelFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerFactory _loggerFactory;
    private readonly OutgoingContextPropagationFilter _contextFilter;

    public GrpcChannelFactory(
        IServiceProvider serviceProvider,
        ILoggerFactory loggerFactory,
        OutgoingContextPropagationFilter contextFilter)
    {
        _serviceProvider = serviceProvider;
        _loggerFactory = loggerFactory;
        _contextFilter = contextFilter;
    }

    public GrpcChannel CreateChannel(string serviceName)
        => GrpcClientDefaults.CreateChannel($"consul:///{serviceName}", _serviceProvider, _loggerFactory);

    public T CreateClient<T>(string serviceName) where T : IService<T>
        => CreateClient<T>(CreateChannel(serviceName));

    public T CreateClient<T>(GrpcChannel channel) where T : IService<T>
        => MagicOnionClient.Create<T>(channel, [_contextFilter]);
}
