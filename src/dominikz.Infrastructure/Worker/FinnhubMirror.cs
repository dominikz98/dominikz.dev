using dominikz.Domain.Models;
using dominikz.Domain.Structs;
using dominikz.Infrastructure.Clients.Finance;
using dominikz.Infrastructure.Extensions;
using dominikz.Infrastructure.Provider.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace dominikz.Infrastructure.Worker;

public class FinnhubMirror : TimeTriggeredWorker
{
    private readonly FinnhubClient _finnhub;
    private readonly OnVistaClient _onVista;
    private readonly DatabaseContext _context;
    private readonly ILogger<FinnhubMirror> _logger;

    public override CronSchedule[] Schedules { get; } = new CronSchedule[]
    {
        // At 02:55 PM
        new("55 14 * * *"),

        // At 11:55 PM
        new("50 23 * * *")
    };

    public FinnhubMirror(FinnhubClient finnhub,
        OnVistaClient onVista,
        DatabaseContext context,
        ILogger<FinnhubMirror> logger)
    {
        _finnhub = finnhub;
        _onVista = onVista;
        _context = context;
        _logger = logger;
    }

    public override async Task Execute(CancellationToken cancellationToken)
    {
        _finnhub.WaitWhenLimitReached = true;

        var shadows = await AddNewShadows(cancellationToken);
        await AttachTradeFlags(shadows, cancellationToken);

        await _context.AddRangeAsync(shadows, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("{ShadowsCount} shadow(s) created", shadows.Count);
    }

    private async Task<IReadOnlyCollection<FinnhubShadow>> AddNewShadows(CancellationToken cancellationToken)
    {
        var date = DateOnly.FromDateTime(DateTime.Now);
        var from = date.ToDateTime(new TimeOnly(0, 0, 1));
        var to = date.ToDateTime(new TimeOnly(23, 59, 59));

        var existing = await _context.From<FinnhubShadow>()
            .Where(x => x.Date == date)
            .Select(x => new { x.Symbol, x.Date })
            .ToListAsync(cancellationToken);

        var whispers = await _context.From<WhispersShadow>()
            .Where(x => x.Date == date)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var positiveCalls = (await _finnhub.GetEarningsCalendar(from, to, cancellationToken)).EarningsCalendar
            .GroupBy(x => new { x.Symbol, x.Date })
            .Select(x => x.FirstOrDefault(y => y.Hour.Equals("bmo", StringComparison.OrdinalIgnoreCase)) ?? x.First())
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

            // only do more checks if release timestamp is known
            if (call.Release is null)
                continue;

            // only check if release is 15 overdue
            var utcTimestamp = call.Date.ToDateTime(call.Release.Value);
            if (utcTimestamp >= DateTime.UtcNow.AddMinutes(15))
                continue;

            var releaseUtcTimestamp = call.Date.ToDateTime(call.Release!.Value);
            // use release - 4h or ls exchange open + 30min
            var from = new[] { releaseUtcTimestamp.AddHours(-4), call.Date.ToDateTime(new TimeOnly(6, 0, 0)) }.Max();
            // use release + 2h, ls exchange close or now
            var to = new[] { releaseUtcTimestamp.AddHours(2), call.Date.ToDateTime(new TimeOnly(21, 0, 0)), DateTime.UtcNow }.Min();

            var candles = await _finnhub.GetCandles(call.Symbol, from, to, cancellationToken);
            if (candles.Close.Length <= 0)
                continue;

            // only data before or after release
            if (candles.Timestamp.All(x => x.FromUnixTimestamp() < releaseUtcTimestamp)
                || candles.Timestamp.All(x => x.FromUnixTimestamp() > releaseUtcTimestamp))
                continue;

            var start = candles.Close.First();
            var close = candles.Close.Last();
            var percent = (close - start) / Math.Abs(start) * 100;
            call.ChartFlag = percent > 1.5m;
        }
    }
}