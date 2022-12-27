using dominikz.api.Provider;
using dominikz.shared.Contracts;
using ImageMagick;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
    
    [HttpGet("image/{id:guid}/{size}")]
    [ResponseCache(Duration = 604800)]
    public async Task<IActionResult> Execute(Guid id, ImageSizeEnum size, CancellationToken cancellationToken)
    {
        var file = await _mediator.Send(new GetImageQuery(id, size), cancellationToken);
        if (file is null)
            return NotFound();

        return File(file, "image/jpg");
    }
}

public class GetImageQuery : IRequest<Stream?>
{
    public Guid Id { get; set; }
    public ImageSizeEnum Size { get; }

    public GetImageQuery(Guid id, ImageSizeEnum size)
    {
        Id = id;
        Size = size;
    }
}

public class GetImageQueryHandler : IRequestHandler<GetImageQuery, Stream?>
{
    private readonly IStorageProvider _storage;

    public GetImageQueryHandler(IStorageProvider storage)
    {
        _storage = storage;
    }

    public async Task<Stream?> Handle(GetImageQuery request, CancellationToken cancellationToken)
    {
        await using var file = await _storage.Load(request.Id, cancellationToken);
        if (file is null)
            return null;

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
        return ms;
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