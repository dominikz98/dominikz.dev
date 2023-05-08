using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Worker.Contracts;

namespace Worker.Hubs;

internal class CrontabWorkerHub : IDisposable
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _hubLogger;

    private readonly Assembly[] _assemblies;
    private readonly IMemoryCache _cache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

    public CrontabWorkerHub(ILoggerFactory loggerFactory, Assembly[] assemblies)
    {
        _loggerFactory = loggerFactory;
        _hubLogger = _loggerFactory.CreateLogger(nameof(CrontabWorkerHub));
        _assemblies = assemblies;
    }

    public void TryRun(IReadOnlyCollection<CrontabWorker> workerList, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var worker in workerList)
            {
                _hubLogger.LogDebug("{Name} started", worker.GetType().Name);
                var workerLogger = _loggerFactory.CreateLogger(worker.GetType().Name);
                worker.Execute(workerLogger, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            _hubLogger.LogError(ex, "Exception: {ExMessage} StackTrace: {ExStackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public IReadOnlyCollection<CrontabWorker> Poll()
    {
        var crontabWorker = _assemblies.SelectMany(x => x
                .GetTypes()
                .Where(y => typeof(CrontabWorker).IsAssignableFrom(y))
                .Where(y => !y.IsAbstract)
                .ToArray())
            .ToList();

        var active = crontabWorker.Select(x => new { Type = x, Instance = (CrontabWorker?)Activator.CreateInstance(x) })
            .Where(x => x.Instance != null)
            .Where(x => x.Instance!.Schedules.Any(y => y.IsTime(DateTime.Now)))
            .ToList();

        var result = new List<CrontabWorker>();
        foreach (var worker in active)
        {
            // already executed this minute?
            var key = $"{worker.Type.Name}#{DateTime.Now.Minute}";
            if (_cache.TryGetValue(key, out _))
                continue;

            // block worker for 1 minute
            _cache.Set<object?>(key, null, DateTimeOffset.UtcNow.AddMinutes(1));
            result.Add(worker.Instance!);
        }

        return result;
    }

    public IReadOnlyCollection<string> List()
        => _assemblies.SelectMany(x => x
                .GetTypes()
                .Where(y => typeof(CrontabWorker).IsAssignableFrom(y))
                .Where(y => !y.IsAbstract)
                .ToArray())
            .Select(x => x.Name)
            .ToList();

    public void Dispose()
        => _cache.Dispose();
}