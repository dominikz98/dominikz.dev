using ImageMagick;

namespace dominikz.Infrastructure.Provider.Storage.Requests;

public class UploadLogoRequest : IStorageUploadRequest
{
    public string Name { get; }
    public Stream Data { get; }

    public IReadOnlyCollection<IStorageProcessor> Processors { get; }

    public UploadLogoRequest(string symbol, Stream data, MagickFormat inputFormat)
    {
        Name = Path.Combine("logos", $"{symbol.ToUpper()}.png");
        Data = data;
        Processors = new IStorageProcessor[]
        {
            new ResizeImageProcessor(150, 150, inputFormat),
            new ConvertImageProcessor(inputFormat, MagickFormat.Png)
        };
    }
}