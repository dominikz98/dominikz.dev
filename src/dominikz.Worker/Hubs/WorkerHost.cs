using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace dominikz.Worker.Hubs;

internal class WorkerHost : IDisposable
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _logger;

    private readonly CancellationToken _cancellationToken;
    private readonly CrontabWorkerHub _crontabWorkerHub;
    private readonly QueueWorkerHub _queueWorkerHub;

    private const int PollingPeriodInSec = 10;
    private bool _startup = true;

    public WorkerHost(ILoggerFactory loggerFactory, IConfigurationRoot configuration, CancellationToken cancellationToken)
    {
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger(nameof(WorkerHost));

        _cancellationToken = cancellationToken;
        _crontabWorkerHub = new CrontabWorkerHub(_loggerFactory, configuration);
        _queueWorkerHub = new QueueWorkerHub(_loggerFactory, configuration);
    }

    public async Task Start()
    {
        // display registered trigger
        _logger.LogInformation("Crontab-Trigger:");
        foreach (var worker in _crontabWorkerHub.List())
            _logger.LogInformation("-{Worker}", worker);

        _logger.LogInformation("Queue-Trigger:");
        foreach (var worker in QueueWorkerHub.List())
            _logger.LogInformation("-{Worker}", worker);

        // loop polling
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(PollingPeriodInSec));
        while (_startup || _cancellationToken.IsCancellationRequested == false && await timer.WaitForNextTickAsync(_cancellationToken))
        {
            _startup = false;

            try
            {
                _logger.LogDebug("Polling ...");

                // poll and execute crontab worker
                var crontabWorkerList = _crontabWorkerHub.Poll();
                _crontabWorkerHub.TryRun(crontabWorkerList, _cancellationToken);

                // poll and execute queue worker
                var queueWorkerList = _queueWorkerHub.Poll();
                _queueWorkerHub.TryRun(queueWorkerList, _cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Critical Exception: {ExMessage} StackTrace: {ExStackTrace}", e.Message, e.StackTrace);
            }
        }
    }

    public void Dispose()
    {
        _crontabWorkerHub.Dispose();
        _queueWorkerHub.Dispose();
        _loggerFactory.Dispose();
    }
}