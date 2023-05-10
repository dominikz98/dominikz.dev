using dominikz.Domain.Enums;
using dominikz.Domain.Models;
using dominikz.Domain.Structs;
using dominikz.Infrastructure.Clients;
using dominikz.Infrastructure.Clients.Noobit;
using dominikz.Infrastructure.Provider.Database;
using dominikz.Infrastructure.Provider.Storage;
using dominikz.Infrastructure.Provider.Storage.Requests;
using dominikz.Worker.Contracts;
using ImageMagick;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace dominikz.Worker.Worker;

public class ExternalArticleShadowCrontabWorker : CrontabWorker
{
    public override CronSchedule[] Schedules { get; } = new CronSchedule[]
    {
        // At 06:00
        new("0 5 * * *")
    };

    protected override async Task Execute(ILogger logger, IConfigurationRoot configuration, CancellationToken cancellationToken)
        => await new ServiceCollection()
            .AddScoped<ILogger>(_ => logger)
            .AddWorkerOptions(configuration)
            .AddProvider(configuration)
            .AddExternalClients()
            .AddScoped<ExternalArticleShadowMirror>()
            .BuildServiceProvider()
            .GetRequiredService<ExternalArticleShadowMirror>()
            .Execute(cancellationToken);
}

public class ExternalArticleShadowMirror
{
    private readonly DatabaseContext _database;
    private readonly IStorageProvider _storage;
    private readonly NoobitClient _noobitClient;
    private readonly MedlanClient _medlanClient;
    private readonly ILogger _logger;

    public ExternalArticleShadowMirror(DatabaseContext database,
        IStorageProvider storage,
        NoobitClient noobitClient,
        MedlanClient medlanClient,
        ILogger logger)
    {
        _database = database;
        _storage = storage;
        _noobitClient = noobitClient;
        _medlanClient = medlanClient;
        _logger = logger;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        var existing = await _database.From<ExtArticleShadow>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var shadows = new List<ExtArticleShadow>();
        var noobitShadows = await _noobitClient.GetArticles(cancellationToken);
        var medlanShadows = await _medlanClient.GetArticles(cancellationToken);

        foreach (var shadow in noobitShadows.Union(medlanShadows))
        {
            var exits = existing.Any(x => x.Title == shadow.Title && x.Date == shadow.Date);
            if (exits)
                continue;

            shadows.Add(shadow);

            if (shadow.Image is null || shadow.ImageId == Guid.Empty)
                continue;

            await _storage.Upload(new UploadImageRequest(shadow.ImageId, shadow.Image, MagickFormat.Unknown, ImageSizeEnum.ThumbnailHorizontal), default);
        }

        await _database.AddRangeAsync(shadows, cancellationToken);
        await _database.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{ShadowsCount} article(s) created", shadows.Count);
    }
}