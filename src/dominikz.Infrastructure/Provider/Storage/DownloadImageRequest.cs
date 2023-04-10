using dominikz.Domain.Enums;

namespace dominikz.Infrastructure.Provider.Storage;

public class DownloadImageRequest : IStorageDownloadRequest
{
    public string Name { get; }
    public bool UseAsync => false;
    public IReadOnlyCollection<IStorageProcessor> Processors { get; } = Array.Empty<IStorageProcessor>();

    public DownloadImageRequest(Guid id, ImageSizeEnum? size = null)
    {
        Name = Path.Combine("images", $"{id}.jpg".ToLower());

        if (size is null or ImageSizeEnum.Original)
            return;

        var (width, height) = CalculateSize(size.Value);
        Processors = new[]
        {
            new ResizeImageProcessor(width, height)
        };
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