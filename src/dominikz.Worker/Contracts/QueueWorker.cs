using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace dominikz.Worker.Contracts;

public abstract class QueueWorker
{
    public abstract DateTime? Timestamp { get; }

    public async Task ExecuteInternal(ILogger logger, IConfigurationRoot configuration, string? payload, CancellationToken cancellationToken)
    {
        try
        {
            await Execute(logger, configuration, payload, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception: {ExMessage} StackTrace: {ExStackTrace}", ex.Message, ex.StackTrace);
        }
    }

    protected abstract Task Execute(ILogger logger, IConfigurationRoot configuration, string? payload, CancellationToken cancellationToken);
}