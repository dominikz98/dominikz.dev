namespace dominikz.Infrastructure.Provider.Storage.Requests;

public class DownloadLogoRequest : IStorageDownloadRequest
{
    public string Name { get; }
    public bool UseAsync => false;
    public IReadOnlyCollection<IStorageProcessor> Processors { get; } = Array.Empty<IStorageProcessor>();

    public DownloadLogoRequest(string symbol)
    {
        Name = Path.Combine("logos", $"{symbol.ToUpper()}.png");
    }
}