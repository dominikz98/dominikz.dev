using dominikz.Domain.Models;
using dominikz.Domain.Structs;

namespace dominikz.Api.Background;

public class TestService : ITimeTriggeredWorker
{
    public CronSchedule[] Schedules { get; } = new CronSchedule[]
    {
        // At 07:01
        new("15 20 * * *")
    };
    
    public Task<bool> Execute(WorkerLog log, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}