using dominikz.dev.Components.Files;
using dominikz.shared.Enums;

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

    public async Task<FileStruct?> RawImage(string url, CancellationToken cancellationToken = default)
    {
        var client = new HttpClient();
        var response = await client.GetAsync(url, HttpCompletionOption.ResponseContentRead, cancellationToken);
        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var contentType = response.Content.Headers.ContentType?.ToString();
        if (contentType is null)
            return null;

        return new FileStruct(Guid.NewGuid().ToString(), contentType, stream);
    }
}