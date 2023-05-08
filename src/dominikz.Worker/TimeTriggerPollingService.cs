using System.Diagnostics;
using dominikz.Infrastructure.Clients;
using dominikz.Infrastructure.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace dominikz.Worker;

public class TimeTriggerPollingService
{
    private const int PeriodInSec = 59;
    private const int RetryCount = 5;
    private const int RetrySleepInSec = 30;

    private readonly ILogger<TimeTriggerPollingService> _logger;
    private readonly IServiceProvider _services;
    private readonly EmailClient _email;
    private readonly IReadOnlyCollection<Type> _worker;
    private bool _startup = true;

    public TimeTriggerPollingService(ILogger<TimeTriggerPollingService> logger,
        IServiceProvider services,
        EmailClient email,
        IReadOnlyCollection<Type> worker)
    {
        _logger = logger;
        _services = services;
        _email = email;
        _worker = worker;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(PeriodInSec));
        _logger.LogInformation("Polling Worker started");

        while (_startup || cancellationToken.IsCancellationRequested == false && await timer.WaitForNextTickAsync(cancellationToken))
        {
            _startup = false;
            _logger.LogDebug($"Polling Worker Schedules");

            foreach (var worker in _worker)
                try
                {
                    StartWorkerWithRetryPolicyIfRequired(worker, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Critical Exception: {ExMessage} StackTrace: {ExStackTrace}", ex.Message, ex.StackTrace);
                    _email.Send($"WORKER - {worker.Name}: Exception", ex);
                }

            await Task.Delay(1000, cancellationToken);
        }

        _logger.LogInformation("Polling Worker stopped");
    }

    private async void StartWorkerWithRetryPolicyIfRequired(Type worker, CancellationToken cancellationToken)
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
                var sw = new Stopwatch();
                sw.Start();
                _logger.LogInformation("Start Executing: {Name}", worker.Name);
                await Task.Run(async () => await instance.Execute(cancellationToken), cancellationToken);
                sw.Stop();
                _logger.LogInformation("Finished Executing: {Name} in {Seconds} seconds", worker.Name, sw.Elapsed.TotalSeconds);
            });
    }
}