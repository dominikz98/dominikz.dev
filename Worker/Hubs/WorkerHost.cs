using System.Reflection;
using dominikz.Infrastructure.Worker;
using Microsoft.Extensions.Logging;

namespace Worker.Hubs;

internal class WorkerHost : IDisposable
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _logger;

    private readonly CancellationToken _cancellationToken;
    private readonly CrontabWorkerHub _crontabWorkerHub;
    private readonly QueueWorkerHub _queueWorkerHub;

    private const int PollingPeriodInSec = 10;
    private bool _startup = true;
    private readonly Assembly[] _assemblies = new[] { typeof(WhispersMirror).Assembly, typeof(WorkerHost).Assembly };

    public WorkerHost(ILoggerFactory loggerFactory, CancellationToken cancellationToken)
    {
        _loggerFactory = loggerFactory;
        _cancellationToken = cancellationToken;
        _logger = _loggerFactory.CreateLogger(nameof(WorkerHost));
        _crontabWorkerHub = new CrontabWorkerHub(_loggerFactory, _assemblies);
        _queueWorkerHub = new QueueWorkerHub(_loggerFactory);
    }

    public async Task Start()
    {
        _logger.LogInformation("Crontab-Trigger:");
        foreach (var worker in _crontabWorkerHub.List())
            _logger.LogInformation("-{Worker}", worker);

        _logger.LogInformation("Queue-Trigger:");
        foreach (var worker in QueueWorkerHub.List())
            _logger.LogInformation("-{Worker}", worker);

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(PollingPeriodInSec));
        while (_startup || _cancellationToken.IsCancellationRequested == false && await timer.WaitForNextTickAsync(_cancellationToken))
        {
            _startup = false;

            try
            {
                _logger.LogDebug("Polling ...");

                var crontabWorkerList = _crontabWorkerHub.Poll();
                _crontabWorkerHub.TryRun(crontabWorkerList, _cancellationToken);

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