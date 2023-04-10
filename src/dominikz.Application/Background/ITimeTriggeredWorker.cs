using dominikz.Domain.Models;
using dominikz.Domain.Structs;

namespace dominikz.Application.Background;

public interface ITimeTriggeredWorker
{
    CronSchedule Schedule { get; }

    Task<bool> Execute(WorkerLog log, CancellationToken cancellationToken);
}