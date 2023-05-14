using dominikz.Domain.Enums;
using dominikz.Domain.Models;
using dominikz.Domain.Structs;
using dominikz.Infrastructure.Clients.Finance;
using dominikz.Infrastructure.Extensions;
using dominikz.Infrastructure.Provider.Database;
using dominikz.Worker.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace dominikz.Worker.Worker.Trading;

public class CurrentStockPriceCrontabWorker : CrontabWorker
{
    public override CronSchedule[] Schedules { get; } = new CronSchedule[]
    {
        // Every 15 minutes, between 05:00 AM UTC and 09:59 PM UTC, Monday through Friday
        new("*/15 5-21 * * 1-5")
    };

    protected override async Task Execute(ILogger logger, IConfigurationRoot configuration, CancellationToken cancellationToken)
    {
        using var scope = new ServiceCollection()
            .AddScoped<ILogger>(_ => logger)
            .AddWorkerOptions(configuration)
            .AddProvider(configuration)
            .AddExternalClients()
            .AddScoped<CurrentStockPriceCollector>()
            .BuildServiceProvider()
            .CreateScope();

        await scope.ServiceProvider
            .GetRequiredService<CurrentStockPriceCollector>()
            .Execute(cancellationToken);
    }
}

public class CurrentStockPriceCollector
{
    private readonly ILogger _logger;
    private readonly DatabaseContext _database;
    private readonly FinnhubClient _finnhub;

    public CurrentStockPriceCollector(ILogger logger, DatabaseContext database, FinnhubClient finnhub)
    {
        _logger = logger;
        _database = database;
        _finnhub = finnhub;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        _finnhub.WaitWhenLimitReached = true;

        var nowPlus1H = DateTime.UtcNow.AddHours(1).ToUnixTimestamp();
        var (todayStart, _) = DateOnly.FromDateTime(DateTime.UtcNow).ToUnixRange();
        var (yesterdayStart, _) = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)).ToUnixRange();

        var calls = await _database.From<EarningCall>()
            .Where(x => (x.UtcTimestamp >= todayStart && x.UtcTimestamp <= nowPlus1H) // today, but finance release already finished or 1 hour before
                        || (x.Time == EarningCallTime.AMC && x.UtcTimestamp >= yesterdayStart && x.UtcTimestamp < todayStart)) // yesterday, but after market closing
            .Select(x => new { x.Id, x.Symbol })
            .ToListAsync(cancellationToken);

        var counter = 0;
        foreach (var call in calls)
        {
            var quote = await _finnhub.GetQuoteBySymbol(call.Symbol, cancellationToken);
            if ((quote?.Current ?? 0) <= 0)
                continue;

            counter++;
            await _database.AddAsync(new StockPrice()
            {
                EarningCallId = call.Id,
                UtcTimestamp = DateTime.UtcNow.ToUnixTimestamp(),
                Value = quote!.Current
            }, cancellationToken);
        }

        await _database.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("[{Timestamp:HH:mm:ss}]: {Count} stock prices saved", DateTime.UtcNow, counter);
    }
}