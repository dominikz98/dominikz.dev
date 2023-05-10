using dominikz.Domain.Structs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace dominikz.Worker.Contracts;

public abstract class CrontabWorker
{
    public abstract CronSchedule[] Schedules { get; }

    public async Task ExecuteInternal(ILogger logger, IConfigurationRoot configuration, CancellationToken cancellationToken)
    {
        try
        {
            await Execute(logger, configuration, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception: {ExMessage} StackTrace: {ExStackTrace}", ex.Message, ex.StackTrace);
        }
    }

    protected abstract Task Execute(ILogger logger, IConfigurationRoot configuration, CancellationToken cancellationToken);
}