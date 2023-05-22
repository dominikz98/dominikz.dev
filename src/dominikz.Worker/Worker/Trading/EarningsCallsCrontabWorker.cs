using dominikz.Domain.Enums;
using dominikz.Domain.Models;
using dominikz.Domain.Structs;
using dominikz.Infrastructure.Clients.Finance;
using dominikz.Infrastructure.Excel;
using dominikz.Infrastructure.Extensions;
using dominikz.Infrastructure.Provider.Database;
using dominikz.Worker.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace dominikz.Worker.Worker.Trading;

public class EarningsCallsCrontabWorker : CrontabWorker
{
    public override CronSchedule[] Schedules { get; } = new CronSchedule[]
    {
        // https://crontab.cronhub.io/

        // At 04:30 AM UTC, Monday through Friday
        new("30 4 * * 1-5"),
        // At 13:14 AM UTC, Monday through Friday
        new("15 13 * * 1-5"),
        // At 21:55 AM UTC, Monday through Friday
        new("55 21 * * 1-5")
    };

    protected override async Task Execute(ILogger logger, IConfigurationRoot configuration, CancellationToken cancellationToken)
    {
        using var scope = new ServiceCollection()
            .AddScoped<ILogger>(_ => logger)
            .AddWorkerOptions(configuration)
            .AddProvider(configuration)
            .AddExternalClients()
            .AddScoped<EarningsCallsCollector>()
            .BuildServiceProvider()
            .CreateScope();

        await scope.ServiceProvider
            .GetRequiredService<EarningsCallsCollector>()
            .Execute(cancellationToken);
    }
}

public class EarningsCallsCollector
{
    private readonly ILogger _logger;
    private readonly TradingProtocolExcel _protocolExcel;
    private readonly DatabaseContext _database;
    private readonly OnVistaClient _onVista;
    private readonly FinnhubClient _finnhub;
    private readonly EarningsWhispersClient _whispers;

    public EarningsCallsCollector(ILogger logger,
        TradingProtocolExcel protocolExcel,
        DatabaseContext database,
        OnVistaClient onVista,
        FinnhubClient finnhub,
        EarningsWhispersClient whispers)
    {
        _logger = logger;
        _protocolExcel = protocolExcel;
        _database = database;
        _onVista = onVista;
        _finnhub = finnhub;
        _whispers = whispers;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        // get earnings whispers
        var whispersCalls = (await _whispers.GetEarningsCallsOfToday())
            .Where(x => x.Release != null || x.Eps != null)
            .ToList();

        if (whispersCalls.Count == 0)
            return;

        var (todayStart, todayEnd) = DateOnly.FromDateTime(DateTime.UtcNow).ToUnixRange();
        var databaseCalls = await _database.From<EarningCall>()
            .Where(x => x.UtcTimestamp >= todayStart && x.UtcTimestamp <= todayEnd)
            .ToListAsync(cancellationToken);

        var addCounter = 0;
        var updCounter = 0;
        foreach (var whisperCall in whispersCalls)
        {
            var databaseCall = databaseCalls.FirstOrDefault(x => x.Symbol == whisperCall.Symbol);
            if (databaseCall != null)
            {
                updCounter++;
                databaseCall.EpsFlag = whisperCall.Eps is null ? null : whisperCall.Eps.Value > 0;
                databaseCall.RevenueFlag = whisperCall.Revenue is null ? null : whisperCall.Revenue.Value > 0;
                databaseCall.Surprise = whisperCall.Surprise;
                databaseCall.Growth = whisperCall.Growth;
                _database.Update(databaseCall);
            }
            else if (whisperCall.Release != null)
            {
                var (time, release) = GetTimestamps(whisperCall);
                if (time == EarningCallTime.DMO)
                    // skip during market is open
                    continue;

                addCounter++;
                var company = await _finnhub.GetCompany(whisperCall.Symbol, cancellationToken);
                var ovStock = await _onVista.SearchStockBySymbol(whisperCall.Symbol, cancellationToken);

                await _database.AddAsync(new EarningCall()
                {
                    Symbol = whisperCall.Symbol,
                    Company = company?.Name ?? ovStock?.Name ?? string.Empty,
                    ISIN = ovStock?.ISIN ?? string.Empty,
                    UtcTimestamp = release.ToUnixTimestamp(),
                    Time = time
                }, cancellationToken);
            }
        }

        await _database.SaveChangesAsync(cancellationToken);
        await CreateExcelSheet(cancellationToken);
        _logger.LogInformation("[{Timestamp:HH:mm:ss}]: {AddCount} created. {UpdCount} updated", DateTime.UtcNow, addCounter, updCounter);
    }

    private async Task CreateExcelSheet(CancellationToken cancellationToken)
    {
        var calls = await _database.From<EarningCall>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        _protocolExcel.Create(calls);
    }

    private (EarningCallTime Time, DateTime Release) GetTimestamps(EwCall call)
    {
        var release = DateOnly.FromDateTime(DateTime.UtcNow).ToDateTime(call.Release!.Value, DateTimeKind.Utc);

        EarningCallTime time;
        if (release < DateOnly.FromDateTime(DateTime.UtcNow).ToDateTime(new TimeOnly(13, 30, 0)))
            time = EarningCallTime.BMO;
        else if (release >= DateOnly.FromDateTime(DateTime.UtcNow).ToDateTime(new TimeOnly(20, 0, 0)))
            time = EarningCallTime.AMC;
        else
            time = EarningCallTime.DMO;

        return (time, release);
    }

    // private async Task<bool> TryUploadLogo(string symbol, FhCompany? company, CancellationToken cancellationToken)
    // {
    //     if (company == null)
    //         return false;
    //
    //     var logo = await LogoRetriever.GetLogoAsStream(company.Logo, cancellationToken);
    //     logo ??= await LogoRetriever.GetFaviconAsStream(company.Weburl, cancellationToken);
    //     if (logo == null)
    //         return false;
    //
    //     await _storage.Upload(new UploadLogoRequest(symbol, logo, MagickFormat.Jpg), cancellationToken);
    //     return true;
    // }
}