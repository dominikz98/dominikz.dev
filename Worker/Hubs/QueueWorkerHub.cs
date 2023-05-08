using System.Text.Json;
using Microsoft.Extensions.Logging;
using Worker.Contracts;
using Worker.Utils;

namespace Worker.Hubs;

internal class QueueWorkerHub : IDisposable
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _hubLogger;

    private const string StackFileName = "trigger_stack.tmp";
    private static List<QueueWorkerEntry> _queue = new();

    public QueueWorkerHub(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _hubLogger = loggerFactory.CreateLogger(nameof(QueueWorkerHub));
        RestoreQueue();
    }

    public void TryRun(IReadOnlyCollection<QueueWorkerEntry> entriesList, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var entry in entriesList)
            {
                _hubLogger.LogDebug("{Name} started", entry.GetType().Name);
                var workerLogger = _loggerFactory.CreateLogger(entry.GetType().Name);
                entry.Worker.Execute(workerLogger, entry.Payload, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            _hubLogger.LogError(ex, "Exception: {ExMessage} StackTrace: {ExStackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public IReadOnlyCollection<QueueWorkerEntry> Poll()
    {
        var active = _queue.Where(x => x.Worker.Timestamp == null || x.Worker.Timestamp.Value.IsLowerOrEqualWithoutSeconds(DateTime.Now)).ToList();
        _queue = _queue.Except(active).ToList();
        return active;
    }

    public static IReadOnlyCollection<string> List()
        => _queue.Select(x => x.Worker.GetType().Name).ToList();

    public static void Add(QueueWorker worker, string? payload)
    {
        _queue.Add(new QueueWorkerEntry() { Worker = worker, Payload = payload });
    }

    private void StoreQueue()
    {
        var json = JsonSerializer.Serialize(_queue);
        File.WriteAllText(StackFileName, json);
        _hubLogger.LogDebug("Queue stored");
    }

    private void RestoreQueue()
    {
        try
        {
            if (_queue.Count > 0)
                return;

            if (File.Exists(StackFileName) == false)
                return;

            var json = File.ReadAllText(StackFileName);
            _queue = JsonSerializer.Deserialize<List<QueueWorkerEntry>>(json) ?? new List<QueueWorkerEntry>();
            _hubLogger.LogDebug("Queue restored");
        }
        catch (JsonException)
        {
        }
    }

    public void Dispose()
        => StoreQueue();
}

internal class QueueWorkerEntry
{
#pragma warning disable CS8618
    public QueueWorker Worker { get; set; }
#pragma warning restore CS8618
    public string? Payload { get; set; }
}