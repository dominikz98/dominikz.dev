using dominikz.Infrastructure.Provider.Storage.Requests;
using ImageMagick;

namespace dominikz.Infrastructure.Provider.Storage;

internal class ConvertImageProcessor : IStorageProcessor
{
    private readonly MagickFormat _inputFormat;
    private readonly MagickFormat _outputFormat;

    public ConvertImageProcessor(MagickFormat inputFormat, MagickFormat outputFormat)
    {
        _inputFormat = inputFormat;
        _outputFormat = outputFormat;
    }

    public async Task<Stream> Execute(Stream data, CancellationToken cancellationToken)
    {
        try
        {
            if (data.CanSeek)
                data.Position = 0;

            var image = new MagickImage(data, _inputFormat);
            image.Format = _outputFormat;

            var ms = new MemoryStream();
            await image.WriteAsync(ms, cancellationToken);
            ms.Position = 0;
            return ms;
        }
        catch (MagickImageErrorException)
        {
            return data;
        }
        catch (MagickCorruptImageErrorException)
        {
            return data;
        }
    }
}