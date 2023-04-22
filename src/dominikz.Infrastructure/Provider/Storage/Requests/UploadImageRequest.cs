using dominikz.Domain.Enums;
using ImageMagick;

namespace dominikz.Infrastructure.Provider.Storage.Requests;

public class UploadImageRequest : IStorageUploadRequest
{
    public string Name { get; }
    public Stream Data { get; }

    public IReadOnlyCollection<IStorageProcessor> Processors { get; } = new[]
    {
        new ConvertImageProcessor(MagickFormat.Jpg)
    };

    public UploadImageRequest(Guid id, Stream data, ImageSizeEnum size = ImageSizeEnum.Original)
    {
        var path = size switch
        {
            ImageSizeEnum.Original => "images",
            ImageSizeEnum.ThumbnailHorizontal => "thumbnails",
            ImageSizeEnum.ThumbnailVertical => "thumbnails",
            _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
        };

        Name = Path.Combine(path, $"{id}.jpg".ToLower());
        Data = data;

        if (size == ImageSizeEnum.Original)
            return;

        var processors = Processors.ToList();
        var (width, height) = size switch
        {
            ImageSizeEnum.ThumbnailHorizontal => (300, 160),
            ImageSizeEnum.ThumbnailVertical => (140, 210),
            _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
        };
        processors.Add(new ResizeImageProcessor(width, height));
        Processors = processors;
    }
}