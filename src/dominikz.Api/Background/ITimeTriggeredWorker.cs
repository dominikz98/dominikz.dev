using dominikz.Domain.Models;
using dominikz.Domain.Structs;

namespace dominikz.Api.Background;

public interface ITimeTriggeredWorker
{
    CronSchedule[] Schedules { get; }

    Task<bool> Execute(WorkerLog log, CancellationToken cancellationToken);
}