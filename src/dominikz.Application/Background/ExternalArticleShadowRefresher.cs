using dominikz.Domain.Models;
using dominikz.Domain.Structs;
using dominikz.Infrastructure.Clients;
using dominikz.Infrastructure.Clients.Noobit;
using dominikz.Infrastructure.Provider.Database;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Application.Background;

public class ExternalArticleShadowRefresher : ITimeTriggeredWorker
{
    private readonly DatabaseContext _database;
    private readonly NoobitClient _noobitClient;
    private readonly MedlanClient _medlanClient;

    // At 06:00 everyday
    public CronSchedule Schedule { get; } = new("0 5 * * *");

    public ExternalArticleShadowRefresher(DatabaseContext database, NoobitClient noobitClient, MedlanClient medlanClient)
    {
        _database = database;
        _noobitClient = noobitClient;
        _medlanClient = medlanClient;
    }

    public async Task<bool> Execute(WorkerLog log, CancellationToken cancellationToken)
    {
        var existing = await _database.From<ExtArticleShadow>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var shadows = new List<ExtArticleShadow>();
        var noobitShadows = await _noobitClient.GetArticles(cancellationToken);
        var medlanShadows = await _medlanClient.GetArticles(cancellationToken);
        foreach (var article in noobitShadows.Union(medlanShadows))
        {
            var exits = existing.Any(x => x.Title == article.Title && x.Date == article.Date);
            if (exits)
                continue;
            
            shadows.Add(article);
        }

        if (shadows.Count == 0)
            return true;

        log.Log += $"{shadows.Count} article(s) added";
        await _database.AddRangeAsync(shadows, cancellationToken);
        await _database.SaveChangesAsync(cancellationToken);
        return true;
    }
}