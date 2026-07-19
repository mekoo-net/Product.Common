using MagicOnion;

namespace Meeko.Contracts.Scheduling;

/// <summary>
/// Generic scheduled-task entry point. Each domain service implements this interface
/// and dispatches the task key to its own handlers. The caller (scheduler) only needs
/// a service identity and a task key; how the service is discovered is not part of this contract.
/// </summary>
public interface IScheduledTaskRunner : IService<IScheduledTaskRunner>
{
    UnaryResult<ScheduledTaskResult> RunAsync(string taskKey);
}
