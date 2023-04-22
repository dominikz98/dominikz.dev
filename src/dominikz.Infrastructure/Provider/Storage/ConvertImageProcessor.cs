using dominikz.Infrastructure.Provider.Storage.Requests;
using ImageMagick;

namespace dominikz.Infrastructure.Provider.Storage;

internal class ConvertImageProcessor : IStorageProcessor
{
    private readonly MagickFormat _format;

    public ConvertImageProcessor(MagickFormat format)
    {
        _format = format;
    }
    
    public async Task<Stream> Execute(Stream data, CancellationToken cancellationToken)
    {
        if (data.CanSeek)
            data.Position = 0;
        
        var image = new MagickImage(data);
        image.Format = _format;

        var ms = new MemoryStream();
        await image.WriteAsync(ms, cancellationToken);
        ms.Position = 0;
        return ms;
    }
}