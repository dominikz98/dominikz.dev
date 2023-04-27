using dominikz.Domain.Models;
using dominikz.Domain.Structs;
using dominikz.Infrastructure.Clients.Finance;
using dominikz.Infrastructure.Provider.Database;
using Microsoft.Extensions.Logging;

namespace dominikz.Infrastructure.Worker;

public class WhispersMirror : TimeTriggeredWorker
{
    private readonly EarningsWhispersClient _client;
    private readonly DatabaseContext _context;
    private readonly ILogger<WhispersMirror> _logger;

    public override CronSchedule[] Schedules { get; } = new CronSchedule[]
    {
        // At 07:01
        new("1 7 * * *")
    };

    public WhispersMirror(EarningsWhispersClient client,
        DatabaseContext context,
        ILogger<WhispersMirror> logger)
    {
        _client = client;
        _context = context;
        _logger = logger;
    }

    public override async Task Execute(CancellationToken cancellationToken)
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
        _logger.LogInformation("{Count} shadow(s) created", shadows.Count);
    }
}