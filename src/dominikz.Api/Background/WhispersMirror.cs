using dominikz.Domain.Models;
using dominikz.Domain.Structs;
using dominikz.Infrastructure.Clients.Finance;
using dominikz.Infrastructure.Provider.Database;

namespace dominikz.Api.Background;

public class WhispersMirror : ITimeTriggeredWorker
{
    private readonly EarningsWhispersClient _client;
    private readonly DatabaseContext _context;

    public CronSchedule[] Schedules { get; } = new CronSchedule[]
    {
        // At 07:01
        new("1 7 * * *")
    };

    public WhispersMirror(EarningsWhispersClient client, DatabaseContext context)
    {
        _client = client;
        _context = context;
    }

    public async Task<bool> Execute(WorkerLog log, CancellationToken cancellationToken)
    {
        var calls = await _client.GetEarningsCallsOfToday();
        var shadows = calls
            .Where(x => x.Release != null)
            .Select(x => new WhispersShadow
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Release = x.Release!.Value,
                Symbol = x.Symbol
            }).ToList();

        await _context.AddRangeAsync(shadows, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        log.Log = $"{shadows.Count} shadow(s) created.";
        return true;
    }
}