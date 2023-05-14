using dominikz.Worker.Contracts;
using dominikz.Worker.Worker;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace dominikz.Worker.Hubs;

internal class CrontabWorkerHub : IDisposable
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IConfigurationRoot _configuration;
    private readonly ILogger _hubLogger;

    private readonly List<Type> _worker;
    private readonly IMemoryCache _cache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

    public CrontabWorkerHub(ILoggerFactory loggerFactory, IConfigurationRoot configuration)
    {
        _loggerFactory = loggerFactory;
        _configuration = configuration;
        _hubLogger = _loggerFactory.CreateLogger(nameof(CrontabWorkerHub));
        _worker = typeof(ExternalArticleShadowCrontabWorker).Assembly
            .GetTypes()
            .Where(y => typeof(CrontabWorker).IsAssignableFrom(y))
            .Where(y => !y.IsAbstract)
            .Distinct()
            .ToList();
    }

    public void TryRun(IReadOnlyCollection<CrontabWorker> workerList, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var worker in workerList)
            {
                _hubLogger.LogDebug("{Name} started", worker.GetType().Name);
                var workerLogger = _loggerFactory.CreateLogger(worker.GetType().Name);
                worker.ExecuteInternal(workerLogger, _configuration, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            _hubLogger.LogCritical(ex, "Critical Exception: {ExMessage} StackTrace: {ExStackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public IReadOnlyCollection<CrontabWorker> Poll()
    {
        var active = _worker.Select(x => new { Type = x, Instance = (CrontabWorker?)Activator.CreateInstance(x) })
            .Where(x => x.Instance != null)
            .Where(x => x.Instance!.Schedules.Any(y => y.IsTime(DateTime.UtcNow)))
            .ToList();

        var result = new List<CrontabWorker>();
        foreach (var worker in active)
        {
            // already executed this minute?
            var key = $"{worker.Type.Name}#{DateTime.UtcNow.Minute}";
            if (_cache.TryGetValue(key, out _))
                continue;

            // block worker for 1 minute
            _cache.Set<object?>(key, null, DateTimeOffset.UtcNow.AddMinutes(1));
            result.Add(worker.Instance!);
        }

        return result;
    }

    public IReadOnlyCollection<string> List()
        => _worker.Select(x => x.Name).ToList();

    public void Dispose()
        => _cache.Dispose();
}