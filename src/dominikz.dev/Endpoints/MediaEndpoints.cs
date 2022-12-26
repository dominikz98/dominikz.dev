using dominikz.shared.ViewModels;

namespace dominikz.dev.Endpoints;

public class MediaEndpoints
{
    private readonly ApiClient _client;
    private const string Endpoint = "medias";

    public MediaEndpoints(ApiClient client)
    {
        _client = client;
    }

    public async Task<List<MediaPreviewVM>> GetPreview(CancellationToken cancellationToken = default)
        => await _client.Get<MediaPreviewVM>($"{Endpoint}/preview", cancellationToken);
}
