using dominikz.api.Provider;
using dominikz.kernel.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace dominikz.api.Endpoints.Download;

[Tags("download")]
[ApiController]
[Route("api/download")]
public class GetImage : ControllerBase
{
    private readonly IMediator _mediator;

    public GetImage(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("image/{id:guid}/{size}")]
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
        using var image = await _storage.Load(request.Id, cancellationToken);
        if (image is null)
            return null;

        var (width, height) = CalculateSize(request.Size);
        return Resize(image, width, height, SKFilterQuality.High);
    }

    private static (int width, int height) CalculateSize(ImageSizeEnum size)
        => size switch
        {
            ImageSizeEnum.Horizonal => (300, 160),
            ImageSizeEnum.Vertical => (140, 210),
            ImageSizeEnum.Carousel => (180, 270),
            ImageSizeEnum.Avatar => (100, 100),
            ImageSizeEnum.Poster => (220, 330),
            _ => (300, 160)
        };

    private static Stream Resize(Stream image, int maxWidth, int maxHeight, SKFilterQuality quality = SKFilterQuality.Medium)
    {
        using SKBitmap sourceBitmap = SKBitmap.Decode(image);

        var height = Math.Min(maxHeight, sourceBitmap.Height);
        var width = Math.Min(maxWidth, sourceBitmap.Width);

        using SKBitmap scaledBitmap = sourceBitmap.Resize(new SKImageInfo(width, height), quality);
        using SKImage scaledImage = SKImage.FromBitmap(scaledBitmap);
        using SKData data = scaledImage.Encode();

        var ms = new MemoryStream();
        data.SaveTo(ms);
        ms.Position = 0;

        return ms;
    }
}