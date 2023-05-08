using Microsoft.Extensions.Logging;

namespace Worker.Contracts;

public abstract class QueueWorker
{
    public abstract DateTime? Timestamp { get; }

    public abstract Task Execute(ILogger logger, string? payload, CancellationToken cancellationToken);
}