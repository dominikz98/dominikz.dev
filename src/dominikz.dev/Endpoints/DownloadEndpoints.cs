using dominikz.dev.Components.Files;
using dominikz.shared.Contracts;

namespace dominikz.dev.Endpoints;

public class DownloadEndpoints
{
    private readonly ApiClient _client;
    private const string Endpoint = "download";

    public DownloadEndpoints(ApiClient client)
    {
        _client = client;
    }

    public async Task<FileStruct?> Image(Guid imageId, bool suppressCache, ImageSizeEnum size, CancellationToken cancellationToken = default)
        => await _client.Download($"{Endpoint}/image/{(suppressCache ? "fresh/" : string.Empty)}{imageId}/{size}", imageId.ToString(), cancellationToken);
}