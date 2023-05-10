using dominikz.Domain.Enums;
using dominikz.Domain.Models;
using dominikz.Domain.Structs;
using dominikz.Infrastructure.Clients.Finance;
using dominikz.Infrastructure.Provider.Database;
using dominikz.Worker.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace dominikz.Worker.Worker;

public class EarningsDatesCrontabWorker : CrontabWorker
{
    public override CronSchedule[] Schedules { get; } = new CronSchedule[]
    {
        // At 06:30 AM, Monday through Friday
        // https://crontab.cronhub.io/
        new("30 6 * * 1-5")
    };

    protected override async Task Execute(ILogger logger, IConfigurationRoot configuration, CancellationToken cancellationToken)
        => await new ServiceCollection()
            .AddScoped<ILogger>(_ => logger)
            .AddWorkerOptions(configuration)
            .AddProvider(configuration)
            .AddExternalClients()
            .AddScoped<EarningsDatesCollector>()
            .BuildServiceProvider()
            .GetRequiredService<EarningsDatesCollector>()
            .Execute(cancellationToken);
}

public class EarningsDatesCollector
{
    private readonly ILogger _logger;
    private readonly FinnhubClient _finnhub;
    private readonly EarningsWhispersClient _whispers;
    private readonly DatabaseContext _database;

    public EarningsDatesCollector(ILogger logger, FinnhubClient finnhub, EarningsWhispersClient whispers, DatabaseContext database)
    {
        _logger = logger;
        _finnhub = finnhub;
        _whispers = whispers;
        _database = database;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        _finnhub.WaitWhenLimitReached = true;
        
        // get calls from finnhub and earnings whispers
        var whispersCalls = (await _whispers.GetEarningsCallsOfToday())
            .Where(x => x.Release != null)
            .ToList();

        if (whispersCalls.Count == 0)
            return;

        var counter = 0;
        foreach (var call in whispersCalls)
        {
            // check for fundamental stock data
            var quote = await _finnhub.GetQuoteBySymbol(call.Symbol, cancellationToken);
            if ((quote?.Current ?? 0) == 0)
                continue;

            counter++;
            var release = DateOnly.FromDateTime(DateTime.Now).ToDateTime(call.Release!.Value, DateTimeKind.Utc).ToLocalTime();

            EarningCallTime time;
            if (release < DateOnly.FromDateTime(DateTime.Now).ToDateTime(new TimeOnly(15, 30, 0)))
                time = EarningCallTime.BMO;
            else if (release > DateOnly.FromDateTime(DateTime.Now).ToDateTime(new TimeOnly(22, 0, 0)))
                time = EarningCallTime.AMC;
            else
                // skip during market is open
                continue;

            await _database.AddAsync(new EarningCall()
            {
                Symbol = call.Symbol,
                Timestamp = release,
                Time = time
            }, cancellationToken);
        }

        await _database.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Count} calls created", counter);
    }
}