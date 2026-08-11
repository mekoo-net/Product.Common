using MagicOnion;

namespace Meeko.Contracts.Insight;

/// <summary>
/// 产品域 → Observability：凭 trace id 反查这次调用的业务事实（用量 / 计费 / 当时的价格）。
///
/// <para><b>为什么这个契约在平台共享库里。</b> 遥测本身是平台设施，而遥测里没有、也不该有
/// 这些内容——span 只存"这次调用发生了什么"，金额和费率的真源是产品侧那张不可变的定价快照。
/// 所以需要跨进程取的恰好就是"已持久化的那部分"，这类跨服务契约的归属地就是这里，
/// 与 <c>Contracts/Services/{Billing,Keystone,Storage}</c> 同级。</para>
///
/// <para><b>谁实现。</b> 每个业务世界的控制面各实现一份，挂在自己的 InternalRpc 端口上
/// （<c>ProductRpcHosting.MapInternalRpcGroup</c>）。Observability 按世界配 Consul 服务名去找，
/// 没配的世界就不显示业务信息面板。</para>
///
/// <para><b>PII 边界。</b> 实现方<b>不得</b>返回邮箱 / 手机号 / 来源 IP。Observability 是访问面
/// 更宽的内网控制台，身份只给 uid，显示名由它自己去 Keystone 换。
/// 见 <c>Meeko.Observability/docs/trace-propagation.md</c> §5。</para>
/// </summary>
public interface ITraceInsightService : IService<ITraceInsightService>
{
    /// <summary>
    /// 批量查。<b>未命中的 traceId 不出现在结果里</b>——"查不到"和"查到了但金额为 0"是两回事，
    /// 补空位会把前者伪装成后者。调用方按 <see cref="TraceInsight.Ref"/> 对回请求。
    ///
    /// <para>实现方需要自己挡批量上限（约定 200），调用方是内网服务但"内网"不等于"一定守规矩"。</para>
    /// </summary>
    UnaryResult<TraceInsight[]> GetByTraceIdsAsync(string[] traceIds);
}
