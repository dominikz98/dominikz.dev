using dominikz.Infrastructure.Provider.Storage.Requests;
using ImageMagick;

namespace dominikz.Infrastructure.Provider.Storage;

public class ResizeImageProcessor : IStorageProcessor
{
    private readonly int _height;
    private readonly MagickFormat _inputFormat;
    private readonly int _width;

    public ResizeImageProcessor(int width, int height, MagickFormat inputFormat)
    {
        _height = height;
        _inputFormat = inputFormat;
        _width = width;
    }

    public async Task<Stream> Execute(Stream data, CancellationToken cancellationToken)
    {
        try
        {
            if (data.CanSeek)
                data.Position = 0;
        
            using var image = new MagickImage(data, _inputFormat);
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
        catch (MagickImageErrorException e)
        {
            Console.WriteLine(e);
            return data;
        }
        catch (MagickCorruptImageErrorException e)
        {
            Console.WriteLine(e);
            return data;
        }
    }
}