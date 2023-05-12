﻿using dominikz.Domain.Enums;
using dominikz.Domain.Models;
using dominikz.Domain.Structs;
using dominikz.Infrastructure.Clients.Finance;
using dominikz.Infrastructure.Provider.Database;
using dominikz.Worker.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace dominikz.Worker.Worker;

public class CurrentStockPriceCrontabWorker : CrontabWorker
{
    public override CronSchedule[] Schedules { get; } = new CronSchedule[]
    {
        // Every 15 minutes, between 07:00 AM and 11:59 PM, Monday through Friday
        new("*/15 7-23 * * 1-5")
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

        var symbols = await _database.From<EarningCall>()
            .Where(x => x.Timestamp.Date == DateTime.Now.Date && x.Timestamp >= DateTime.Now.AddHours(-1) // today, but finance release already finished or 1 hour before
                        || x.Time == EarningCallTime.AMC && x.Timestamp.Date >= DateTime.Now.AddDays(-1).Date) // yesterday, but after market closing
            .Select(x => x.Symbol)
            .ToListAsync(cancellationToken);

        var counter = 0;
        foreach (var symbol in symbols)
        {
            var quote = await _finnhub.GetQuoteBySymbol(symbol, cancellationToken);
            if ((quote?.Current ?? 0) <= 0)
                continue;

            counter++;
            await _database.AddAsync(new StockPrice()
            {
                Symbol = symbol,
                Timestamp = DateTime.Now,
                Value = quote!.Current
            }, cancellationToken);
        }

        await _database.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("[{Timestamp:HH:mm:ss}]: {Count} stock prices saved", DateTime.Now, counter);
    }
}