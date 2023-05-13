using dominikz.Domain.Enums;
using dominikz.Domain.Models;
using dominikz.Domain.Structs;
using dominikz.Infrastructure.Clients.Finance;
using dominikz.Infrastructure.Provider.Database;
using dominikz.Infrastructure.Provider.Storage;
using dominikz.Infrastructure.Provider.Storage.Requests;
using dominikz.Infrastructure.Utils;
using dominikz.Worker.Contracts;
using ImageMagick;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace dominikz.Worker.Worker.Trading;

public class EarningsDatesCrontabWorker : CrontabWorker
{
    public override CronSchedule[] Schedules { get; } = new CronSchedule[]
    {
        // At 06:30 AM, Monday through Friday
        // https://crontab.cronhub.io/
        new("30 6 * * 1-5")
    };

    protected override async Task Execute(ILogger logger, IConfigurationRoot configuration, CancellationToken cancellationToken)
    {
        using var scope = new ServiceCollection()
            .AddScoped<ILogger>(_ => logger)
            .AddWorkerOptions(configuration)
            .AddProvider(configuration)
            .AddExternalClients()
            .AddScoped<EarningsDatesCollector>()
            .BuildServiceProvider()
            .CreateScope();

        await scope.ServiceProvider
            .GetRequiredService<EarningsDatesCollector>()
            .Execute(cancellationToken);
    }
}

public class EarningsDatesCollector
{
    private readonly ILogger _logger;
    private readonly FinnhubClient _finnhub;
    private readonly EarningsWhispersClient _whispers;
    private readonly OnVistaClient _onVista;
    private readonly StorageProvider _storage;
    private readonly DatabaseContext _database;

    public EarningsDatesCollector(ILogger logger,
        FinnhubClient finnhub,
        EarningsWhispersClient whispers,
        OnVistaClient onVista,
        StorageProvider storage,
        DatabaseContext database)
    {
        _logger = logger;
        _finnhub = finnhub;
        _whispers = whispers;
        _onVista = onVista;
        _storage = storage;
        _database = database;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        _finnhub.WaitWhenLimitReached = true;

        // get earnings whispers
        var whispersCalls = (await _whispers.GetEarningsCallsOfToday())
            .Where(x => x.Release != null)
            .ToList();

        if (whispersCalls.Count == 0)
            return;

        // get earnings calls calender from finnhub
        var finnhubCalls = (await _finnhub.GetEarningsCalendar(DateTime.Now, DateTime.Now, cancellationToken)).EarningsCalendar.ToList();
        if (finnhubCalls.Count == 0)
            return;

        // filter calls
        var finnhubSymbols = finnhubCalls
            .Select(x => x.Symbol)
            .Distinct()
            .ToList();

        var includedInBoth = whispersCalls.Where(x => finnhubSymbols.Contains(x.Symbol)).ToList();

        var counter = 0;
        foreach (var call in includedInBoth)
        {
            var (time, release) = GetTimestamps(call);
            if (time == EarningCallTime.DMO)
                // skip during market is open
                continue;

            // get last 4 finance quarters
            var quarters = (await _finnhub.GetEpsSurprises(call.Symbol, cancellationToken))
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Quarter)
                .ToList();

            if (quarters.Count == 0)
                continue;

            // collect general information
            var company = await _finnhub.GetCompany(call.Symbol, cancellationToken);
            var ovStock = await _onVista.SearchStockBySymbol(call.Symbol, cancellationToken);
            var logoAvailable = await TryUploadLogo(call.Symbol, company, cancellationToken);
            
            counter++;
            await _database.AddAsync(new EarningCall()
            {
                Symbol = call.Symbol,
                Company = company?.Name ?? ovStock?.Name ?? string.Empty,
                ISIN = ovStock?.ISIN ?? string.Empty,
                LogoAvailable = logoAvailable, 
                Timestamp = release,
                Time = time,
                Q1 = quarters.Count > 0 ? quarters[0].Surprise ?? 0 : 0,
                Q2 = quarters.Count > 1 ? quarters[1].Surprise ?? 0 : 0,
                Q3 = quarters.Count > 2 ? quarters[2].Surprise ?? 0 : 0,
                Q4 = quarters.Count > 3 ? quarters[3].Surprise ?? 0 : 0,
            }, cancellationToken);
        }

        await _database.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("[{Timestamp:HH:mm:ss}]: {Count} calls created", DateTime.Now, counter);
    }

    private (EarningCallTime Time, DateTime Release) GetTimestamps(EwCall call)
    {
        var release = DateOnly.FromDateTime(DateTime.Now).ToDateTime(call.Release!.Value, DateTimeKind.Utc).ToLocalTime();

        EarningCallTime time;
        if (release < DateOnly.FromDateTime(DateTime.Now).ToDateTime(new TimeOnly(15, 30, 0)))
            time = EarningCallTime.BMO;
        else if (release >= DateOnly.FromDateTime(DateTime.Now).ToDateTime(new TimeOnly(22, 0, 0)))
            time = EarningCallTime.AMC;
        else
            time = EarningCallTime.DMO;

        return (time, release);
    }

    private async Task<bool> TryUploadLogo(string symbol, FhCompany? company, CancellationToken cancellationToken)
    {
        if (company == null)
            return false;

        var logo = await LogoRetriever.GetLogoAsStream(company.Logo, cancellationToken);
        logo ??= await LogoRetriever.GetFaviconAsStream(company.Weburl, cancellationToken);
        if (logo == null)
            return false;

        await _storage.Upload(new UploadLogoRequest(symbol, logo, MagickFormat.Jpg), cancellationToken);
        return true;
    }
}