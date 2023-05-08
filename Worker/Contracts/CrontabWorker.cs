using dominikz.Domain.Structs;
using Microsoft.Extensions.Logging;

namespace Worker.Contracts;

public abstract class CrontabWorker
{
    public abstract CronSchedule[] Schedules { get; }

    public abstract Task Execute(ILogger logger, CancellationToken cancellationToken);
}