using dominikz.Domain.Models;
using dominikz.Infrastructure.Provider.Database;

namespace dominikz.Api.Background;

class PeriodicHostedService : BackgroundService
{
    private readonly TimeSpan _period = TimeSpan.FromMinutes(7.5);
    private readonly ILogger<PeriodicHostedService> _logger;
    private readonly IServiceScopeFactory _factory;

    public PeriodicHostedService(
        ILogger<PeriodicHostedService> logger,
        IServiceScopeFactory factory)
    {
        _logger = logger;
        _factory = factory;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(_period);

        while (cancellationToken.IsCancellationRequested == false && await timer.WaitForNextTickAsync(cancellationToken))
        {
            try
            {
                await using var scope = _factory.CreateAsyncScope();
                var workerList = scope.ServiceProvider.GetServices<ITimeTriggeredWorker>();
                var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                foreach (var worker in workerList)
                {
                    if (worker.Schedule.IsTime(DateTime.Now) == false)
                        continue;

                    var log = new WorkerLog()
                    {
                        Worker = worker.GetType().Name
                    };
                    
                    try
                    {
                        _logger.LogInformation($"[{nameof(PeriodicHostedService)}] Start Executing: {worker.GetType().Name}");
                        log.Success = await worker.Execute(log, cancellationToken);
                        _logger.LogInformation($"[{nameof(PeriodicHostedService)}] Finished Executing: {worker.GetType().Name}");
                    }
                    catch (Exception e)
                    {
                        database.ChangeTracker.Clear();
                        log.Log ??= string.Empty;
                        log.Log += e.Message + Environment.NewLine;
                        log.Log += e.StackTrace;
                        _logger.LogError($"[{nameof(PeriodicHostedService)}] Failed Executing: {worker.GetType().Name}");
                    }
                    finally
                    {
                        database.ChangeTracker.Clear();
                        await database.AddAsync(log, cancellationToken);
                        await database.SaveChangesAsync(cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(PeriodicHostedService)}] Exception: {ex.Message}");
            }
        }
    }
}