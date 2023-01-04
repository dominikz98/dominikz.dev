using dominikz.api.Models;
using dominikz.api.Models.ViewModels;
using dominikz.api.Provider;
using dominikz.shared.Contracts;
using HeyRed.Mime;
using ImageMagick;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Endpoints.Download;

[Tags("download")]
[Route("api/download")]
public class GetImage : EndpointController
{
    private readonly IMediator _mediator;

    public GetImage(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("image/fresh/{id:guid}/{size}")]
    public async Task<IActionResult> ExecuteWithoutCache(Guid id, ImageSizeEnum size, CancellationToken cancellationToken)
        => await Execute(id, size, cancellationToken);
    
    [HttpGet("image/{id:guid}/{size}")]
    [ResponseCache(Duration = 604800)]
    public async Task<IActionResult> Execute(Guid id, ImageSizeEnum size, CancellationToken cancellationToken)
    {
        var file = await _mediator.Send(new GetImageQuery(id, size), cancellationToken);
        if (file is null)
            return NotFound();

        return File(file.Data, file.ContentType, file.Name);
    }
}

public record GetImageQuery(Guid Id, ImageSizeEnum Size) : IRequest<FileDownloadWrapper?>;

public class GetImageQueryHandler : IRequestHandler<GetImageQuery, FileDownloadWrapper?>
{
    private readonly DatabaseContext _database;
    private readonly IStorageProvider _storage;

    public GetImageQueryHandler(DatabaseContext database, IStorageProvider storage)
    {
        _database = database;
        _storage = storage;
    }

    public async Task<FileDownloadWrapper?> Handle(GetImageQuery request, CancellationToken cancellationToken)
    {
        var info = await _database.From<StorageFile>()
            .AsNoTracking()
            .Select(x => new { x.Id, x.Extension })
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (info == null)
            return null;

        var file = await _storage.Download(request.Id, cancellationToken);
        if (file == null)
            return null;

        var name = $"{info.Id}.{info.Extension}";
        if (request.Size == ImageSizeEnum.Original)
            return new FileDownloadWrapper(file, name, MimeTypesMap.GetMimeType(name));

        using var image = new MagickImage(file);
        var (width, height) = CalculateSize(request.Size);
        var size = new MagickGeometry(width, height)
        {
            IgnoreAspectRatio = true
        };

        image.Resize(size);
        var ms = new MemoryStream();
        await image.WriteAsync(ms, cancellationToken);
        ms.Position = 0;

        return new FileDownloadWrapper(ms, name, MimeTypesMap.GetMimeType(name));
    }

    private static (int width, int height) CalculateSize(ImageSizeEnum size)
        => size switch
        {
            ImageSizeEnum.Horizontal => (300, 160),
            ImageSizeEnum.Vertical => (140, 210),
            ImageSizeEnum.Carousel => (180, 270),
            ImageSizeEnum.Avatar => (100, 100),
            ImageSizeEnum.Poster => (220, 330),
            _ => (300, 160)
        };
}