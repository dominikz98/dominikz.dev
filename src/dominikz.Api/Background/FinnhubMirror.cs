using dominikz.Domain.Models;
using dominikz.Domain.Structs;
using dominikz.Infrastructure.Clients.Finance;
using dominikz.Infrastructure.Provider.Database;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Api.Background;

public class FinnhubMirror : ITimeTriggeredWorker
{
    private readonly FinnhubClient _finnhub;
    private readonly OnVistaClient _onVista;
    private readonly DatabaseContext _context;


    public CronSchedule[] Schedules { get; } = new CronSchedule[]
    {
        // At 02:45 PM
        new("45 14 * * *"),

        // At 11:45 PM
        new("45 23 * * *")
    };

    public FinnhubMirror(FinnhubClient finnhub, OnVistaClient onVista, DatabaseContext context)
    {
        _finnhub = finnhub;
        _finnhub.WithWhenLimitReached = true;
        _onVista = onVista;
        _context = context;
    }

    public async Task<bool> Execute(WorkerLog log, CancellationToken cancellationToken)
    {
        var shadows = await AddNewShadows(cancellationToken);
        await AttachTradeFlags(shadows, cancellationToken);

        log.Log = $"{shadows.Count} shadow(s) created.";
        return true;
    }

    private async Task<IReadOnlyCollection<FinnhubShadow>> AddNewShadows(CancellationToken cancellationToken)
    {
        var date = DateOnly.FromDateTime(DateTime.Now);
        var from = date.ToDateTime(new TimeOnly(0, 0, 1));
        var to = date.ToDateTime(new TimeOnly(23, 59, 59));

        var existing = await _context.From<FinnhubShadow>()
            .Where(x => x.Date == DateOnly.FromDateTime(DateTime.Now))
            .Select(x => new { x.Symbol, x.Date })
            .ToListAsync(cancellationToken);

        var whispers = await _context.From<WhispersShadow>()
            .Where(x => x.Date == date)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var positiveCalls = (await _finnhub.GetEarningsCalendar(from, to, cancellationToken)).EarningsCalendar
            .Where(x => x.EpsActual != null)
            .Where(x => x.EpsEstimate != null)
            .Where(x => x.RevenueActual != null)
            .Where(x => x.RevenueEstimate != null)
            .Where(x => x.EpsActual > x.EpsEstimate)
            .Where(x => x.RevenueActual > x.RevenueEstimate)
            .OrderBy(x => x.Hour)
            .ToList();

        var shadows = new List<FinnhubShadow>();
        foreach (var call in positiveCalls)
        {
            // skip already tracked calls
            var alreadyExists = existing
                .Where(x => x.Date == date)
                .Any(x => x.Symbol == call.Symbol);

            if (alreadyExists)
                continue;

            var epsSurprises = (await _finnhub.GetEpsSurprises(call.Symbol, cancellationToken))
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Quarter)
                .ToList();

            // check for positive surprise
            var current = epsSurprises.FirstOrDefault();
            if (current == null)
                continue;

            // negative surprise -> skip
            if ((current.Surprise ?? 0) <= 0 || (current.SurprisePercent ?? 0) <= 0)
                continue;

            // collect general information
            var name = (await _finnhub.GetCompany(call.Symbol, cancellationToken))?.Name ?? string.Empty;
            var isin = (await _onVista.SearchStockBySymbol(call.Symbol, cancellationToken))?.ISIN;
            var release = whispers.FirstOrDefault(x => x.Symbol == call.Symbol)?.Release;

            shadows.Add(new FinnhubShadow
            {
                Date = date,
                Release = release,
                EpsActual = call.EpsActual!.Value,
                EpsEstimate = call.EpsEstimate!.Value,
                RevenueActual = call.RevenueActual!.Value,
                RevenueEstimate = call.RevenueEstimate!.Value,
                SurprisePercent = current.SurprisePercent!.Value,
                Surprise = current.Surprise!.Value,
                Quarters = epsSurprises.Select(x => x.Actual ?? 0).ToArray(),
                Hour = call.Hour,
                Symbol = call.Symbol,
                Name = name,
                ISIN = isin,
                Updated = DateTime.Now
            });
        }

        await _context.AddRangeAsync(shadows, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return shadows;
    }

    private async Task AttachTradeFlags(IReadOnlyCollection<FinnhubShadow> trackedCalls, CancellationToken cancellationToken)
    {
        foreach (var call in trackedCalls)
        {
            // surprise > 8% flag
            call.IncreaseFlag = call.SurprisePercent > 8;

            // peak over 4 quarters flag
            if (call.Quarters.Length > 0)
                call.PeakFlag = call.Quarters.First() == call.Quarters.Max(x => x);

            if (call.Release is not null)
            {
                // only check if release is 15 overdue
                var utcTimestamp = call.Date.ToDateTime(call.Release.Value);
                if (utcTimestamp < DateTime.UtcNow.AddMinutes(15))
                {
                    var candles = await _finnhub.GetCandles(call.Symbol, utcTimestamp, cancellationToken);
                    var start = candles.Close.First();
                    var close = candles.Close.Last();
                    call.ChartFlag = (close - start) / Math.Abs(start) * 100 > 1;
                }
            }

            _context.Update(call);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}