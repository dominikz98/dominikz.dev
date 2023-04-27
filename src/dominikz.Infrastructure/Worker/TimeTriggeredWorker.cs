using dominikz.Domain.Structs;

namespace dominikz.Infrastructure.Worker;

public abstract class TimeTriggeredWorker
{
    public abstract CronSchedule[] Schedules { get; }

    public abstract Task Execute(CancellationToken cancellationToken);
}