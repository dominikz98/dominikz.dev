// using dominikz.Domain.Models;
// using dominikz.Domain.Structs;
// using dominikz.Infrastructure.Clients.SupermarktCheck;
// using dominikz.Infrastructure.Provider.Database;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
//
// namespace dominikz.Worker.Worker;
//
// public class FoodPriceSnapshotCreator : TimeTriggeredWorker
// {
//     public override CronSchedule[] Schedules { get; } = new CronSchedule[]
//     {
//         // At 02:00 on Monday
//         new("0 2 * * 1")
//     };
//
//     private readonly DatabaseContext _database;
//     private readonly SupermarktCheckClient _client;
//     private readonly ILogger<FoodPriceSnapshotCreator> _logger;
//
//     public FoodPriceSnapshotCreator(DatabaseContext database,
//         SupermarktCheckClient client,
//         ILogger<FoodPriceSnapshotCreator> logger)
//     {
//         _database = database;
//         _client = client;
//         _logger = logger;
//     }
//
//     public override async Task Execute(CancellationToken cancellationToken)
//     {
//         var foods = await _database.From<Food>()
//             .Where(x => x.SupermarktCheckId != null)
//             .Select(x => new { x.Id, x.SupermarktCheckId })
//             .ToListAsync(cancellationToken);
//
//         var snapshots = new List<FoodSnapshot>();
//         foreach (var food in foods)
//         {
//             var prices = (await _client.GetProductById(food.SupermarktCheckId!.Value, cancellationToken))?.Prices ?? Array.Empty<ProductPriceVm>();
//             if (prices.Count == 0)
//                 continue;
//
//             snapshots.AddRange(prices.Select(x => new FoodSnapshot()
//             {
//                 Price = Math.Round(x.Price, 2, MidpointRounding.AwayFromZero),
//                 Store = x.Store,
//                 Timestamp = DateTime.Now,
//                 FoodId = food.Id
//             }));
//
//             await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
//         }
//
//         await _database.AddRangeAsync(snapshots, cancellationToken);
//         await _database.SaveChangesAsync(cancellationToken);
//         _logger.LogInformation("{Count} shadow(s) created", snapshots.Count);
//     }
// }