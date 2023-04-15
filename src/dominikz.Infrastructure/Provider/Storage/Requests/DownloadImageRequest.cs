using dominikz.Domain.Enums;

namespace dominikz.Infrastructure.Provider.Storage.Requests;

public class DownloadImageRequest : IStorageDownloadRequest
{
    public string Name { get; }
    public bool UseAsync => false;
    public IReadOnlyCollection<IStorageProcessor> Processors { get; } = Array.Empty<IStorageProcessor>();

    public DownloadImageRequest(Guid id, ImageSizeEnum size = ImageSizeEnum.Original)
    {
        var path = size switch
        {
            ImageSizeEnum.Original => "images",
            ImageSizeEnum.ThumbnailHorizontal => "thumbnails",
            ImageSizeEnum.ThumbnailVertical => "thumbnails",
            _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
        };

        Name = Path.Combine(path, $"{id}.jpg".ToLower());
    }
}