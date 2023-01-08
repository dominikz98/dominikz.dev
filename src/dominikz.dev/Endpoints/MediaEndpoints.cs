using dominikz.dev.Extensions;
using dominikz.dev.Models;
using dominikz.shared.ViewModels.Media;
using Microsoft.Extensions.Options;

namespace dominikz.dev.Endpoints;

public class MediaEndpoints
{
    private readonly ApiClient _client;
    private readonly IOptions<ApiOptions> _options;
    private const string Endpoint = "medias";

    public MediaEndpoints(ApiClient client, IOptions<ApiOptions> options)
    {
        _client = client;
        _options = options;
    }

    public async Task<List<MediaPreviewVm>> GetPreview(CancellationToken cancellationToken = default)
    {
        var vmList = await _client.Get<MediaPreviewVm>($"{Endpoint}/preview", cancellationToken);
        vmList.AttachApiKey(_options.Value.Key);
        return vmList;
    }
}