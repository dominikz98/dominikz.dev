using dominikz.Infrastructure.Provider.Storage.Requests;
using ImageMagick;

namespace dominikz.Infrastructure.Provider.Storage;

public class ResizeImageProcessor : IStorageProcessor
{
    private readonly int _height;
    private readonly int _width;

    public ResizeImageProcessor(int width, int height)
    {
        _height = height;
        _width = width;
    }

    public async Task<Stream> Execute(Stream data, CancellationToken cancellationToken)
    {
        if (data.CanSeek)
            data.Position = 0;
        
        using var image = new MagickImage(data);
        var size = new MagickGeometry(_width, _height)
        {
            IgnoreAspectRatio = true
        };

        image.Resize(size);
        
        var ms = new MemoryStream();
        await image.WriteAsync(ms, cancellationToken);
        ms.Position = 0;
        
        return ms;
    }
}