namespace dominikz.Application.Background;

class PeriodicHostedService : BackgroundService
{
    private readonly TimeSpan _period = TimeSpan.FromHours(7.5);
    private readonly ILogger<PeriodicHostedService> _logger;
    private readonly IServiceScopeFactory _factory;
    private bool _firstRun = true;

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

        while ( _firstRun || (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken)))
        {
            try
            {
                _firstRun = false;
                await using var scope = _factory.CreateAsyncScope();
                var refresher = scope.ServiceProvider.GetRequiredService<CacheRefresher>();
                await refresher.Refresh(cancellationToken);
                _logger.LogInformation($"[{nameof(PeriodicHostedService)}] Called: {nameof(CacheRefresher)}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(PeriodicHostedService)}] Exception: {ex.Message}");
            }
        }
    }
}