using ImageMagick;

namespace dominikz.Infrastructure.Provider.Storage;

internal class ConvertImageToJpgProcessor : IStorageProcessor
{
    public async Task<Stream> Execute(Stream data, CancellationToken cancellationToken)
    {
        if (data.CanSeek)
            data.Position = 0;
        
        var image = new MagickImage(data);
        image.Format = MagickFormat.Jpg;

        var ms = new MemoryStream();
        await image.WriteAsync(ms, cancellationToken);
        ms.Position = 0;
        return ms;
    }
}