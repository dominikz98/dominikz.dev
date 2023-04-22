using ImageMagick;

namespace dominikz.Infrastructure.Provider.Storage.Requests;

public class UploadLogoRequest : IStorageUploadRequest
{
    public string Name { get; }
    public Stream Data { get; }

    public IReadOnlyCollection<IStorageProcessor> Processors { get; } = new IStorageProcessor[]
    {
        new ResizeImageProcessor(150, 150),
        new ConvertImageProcessor(MagickFormat.Png)
    };

    public UploadLogoRequest(string symbol, Stream data)
    {
        Name = Path.Combine("logos", $"{symbol.ToUpper()}.png");
        Data = data;
    }
}