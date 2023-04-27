using dominikz.Infrastructure.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace dominikz.Worker;

public class TimeTriggerPollingService
{
    private const int PeriodInSec = 30;
    private const int RetryCount = 5;
    private const int RetrySleepInSec = 30;

    private readonly ILogger<TimeTriggerPollingService> _logger;
    private readonly IServiceProvider _services;
    private readonly IReadOnlyCollection<Type> _worker;
    private bool _startup = true;

    public TimeTriggerPollingService(ILogger<TimeTriggerPollingService> logger,
        IServiceProvider services,
        IReadOnlyCollection<Type> worker)
    {
        _logger = logger;
        _services = services;
        _worker = worker;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(PeriodInSec));
        _logger.LogInformation("Polling Worker started");

        while (_startup || cancellationToken.IsCancellationRequested == false && await timer.WaitForNextTickAsync(cancellationToken))
        {
            _startup = false;

            _logger.LogInformation($"Polling Worker Schedules");
            try
            {
                foreach (var worker in _worker)
                    await StartWorkerWithRetryPolicyIfRequired(worker, cancellationToken);

                await Task.Delay(1000, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Critical Exception: {ExMessage}", ex.Message);
            }
        }

        _logger.LogInformation("Polling Worker stopped");
    }

    private async Task StartWorkerWithRetryPolicyIfRequired(Type worker, CancellationToken cancellationToken)
    {
        var instance = (TimeTriggeredWorker)_services.CreateScope().ServiceProvider.GetRequiredService(worker);
        if (instance.Schedules.All(x => x.IsTime(DateTime.Now) == false))
            return;

        await Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                RetryCount,
                _ => TimeSpan.FromSeconds(RetrySleepInSec),
                (exception, timeSpan, retryCount, _) => { _logger.LogWarning("Retry {RetryCount} due to exception {ExceptionMessage}. Waiting {TimeSpan} before retrying", retryCount, exception.Message, timeSpan); })
            .ExecuteAsync(async () =>
            {
                _logger.LogInformation("Start Executing: {Name}", worker.Name);
                await Task.Run(async () => await instance.Execute(cancellationToken), cancellationToken);
                _logger.LogInformation("Finished Executing: {Name}", worker.Name);
            });
    }
}