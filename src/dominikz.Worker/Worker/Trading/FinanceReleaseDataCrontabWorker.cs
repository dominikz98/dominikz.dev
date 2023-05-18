// using dominikz.Domain.Models;
// using dominikz.Domain.Structs;
// using dominikz.Infrastructure.Clients.Finance;
// using dominikz.Infrastructure.Extensions;
// using dominikz.Infrastructure.Provider.Database;
// using dominikz.Worker.Contracts;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
//
// namespace dominikz.Worker.Worker.Trading;
//
// public class FinanceReleaseDataCrontabWorker : CrontabWorker
// {
//     public override CronSchedule[] Schedules { get; } = new CronSchedule[]
//     {
//         // At 21:30 AM UTC, Monday through Friday
//         // https://crontab.cronhub.io/
//         new("30 21 * * 1-5")
//     };
//
//     protected override async Task Execute(ILogger logger, IConfigurationRoot configuration, CancellationToken cancellationToken)
//     {
//         using var scope = new ServiceCollection()
//             .AddScoped<ILogger>(_ => logger)
//             .AddWorkerOptions(configuration)
//             .AddProvider(configuration)
//             .AddExternalClients()
//             .AddScoped<FinanceReleaseDataCollector>()
//             .BuildServiceProvider()
//             .CreateScope();
//
//         await scope.ServiceProvider
//             .GetRequiredService<FinanceReleaseDataCollector>()
//             .Execute(cancellationToken);
//     }
// }
//
// public class FinanceReleaseDataCollector
// {
//     private readonly FinnhubClient _finnhub;
//     private readonly DatabaseContext _database;
//     private readonly ILogger _logger;
//
//     public FinanceReleaseDataCollector(FinnhubClient finnhub, DatabaseContext database, ILogger logger)
//     {
//         _finnhub = finnhub;
//         _database = database;
//         _logger = logger;
//     }
//
//     public async Task Execute(CancellationToken cancellationToken)
//     {
//         var calendar = (await _finnhub.GetEarningsCalendar(DateTime.UtcNow, DateTime.UtcNow, cancellationToken))
//             .EarningsCalendar
//             .Where(x => x.EpsEstimate != null)
//             .Where(x => x.EpsActual != null)
//             .Where(x => x.RevenueEstimate != null)
//             .Where(x => x.RevenueActual != null)
//             .ToList();
//
//         var (todayStart, todayEnd) = DateOnly.FromDateTime(DateTime.UtcNow).ToUnixRange();
//         var calls = await _database.From<EarningCall>()
//             .Where(x => x.UtcTimestamp >= todayStart && x.UtcTimestamp <= todayEnd)
//             .ToListAsync(cancellationToken);
//
//         foreach (var ecEvent in calendar)
//         {
//             var call = calls.FirstOrDefault(x => x.Symbol == ecEvent.Symbol);
//             if (call == null)
//                 continue;
//
//             call.EpsActual = ecEvent.EpsActual!.Value;
//             call.EpsEstimate = ecEvent.EpsEstimate!.Value;
//             call.RevenueActual = ecEvent.RevenueActual!.Value;
//             call.RevenueEstimate = ecEvent.RevenueEstimate!.Value;
//             _database.Update(call);
//         }
//
//         await _database.SaveChangesAsync(cancellationToken);
//         _logger.LogInformation("[{Timestamp:HH:mm:ss}]: financial release data attached to {Count} calls", DateTime.UtcNow, calls.Count(x => x.EpsActual != 0));
//     }
// }