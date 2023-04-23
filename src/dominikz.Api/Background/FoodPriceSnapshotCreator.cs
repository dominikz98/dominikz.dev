using dominikz.Domain.Models;
using dominikz.Domain.Structs;
using dominikz.Infrastructure.Clients.SupermarktCheck;
using dominikz.Infrastructure.Provider.Database;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Api.Background;

public class FoodPriceSnapshotCreator : ITimeTriggeredWorker
{
    // At 02:00 on Monday
    public CronSchedule Schedule { get; } = new("0 2 * * 1");

    private readonly DatabaseContext _database;
    private readonly SupermarktCheckClient _client;

    public FoodPriceSnapshotCreator(DatabaseContext database, SupermarktCheckClient client)
    {
        _database = database;
        _client = client;
    }

    public async Task<bool> Execute(WorkerLog log, CancellationToken cancellationToken)
    {
        var foods = await _database.From<Food>()
            .Where(x => x.SupermarktCheckId != null)
            .Select(x => new { x.Id, x.SupermarktCheckId })
            .ToListAsync(cancellationToken);

        foreach (var food in foods)
        {
            var prices = (await _client.GetProductById(food.SupermarktCheckId!.Value, cancellationToken))?.Prices ?? Array.Empty<ProductPriceVm>();
            if (prices.Count == 0)
                continue;

            await _database.AddRangeAsync(prices.Select(x => new FoodSnapshot()
            {
                Price = Math.Round(x.Price, 2, MidpointRounding.AwayFromZero),
                Store = x.Store,
                Timestamp = DateTime.Now,
                FoodId = food.Id
            }), cancellationToken);

            await _database.SaveChangesAsync(cancellationToken);
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }

        return true;
    }
}