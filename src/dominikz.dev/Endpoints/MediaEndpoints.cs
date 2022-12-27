using dominikz.dev.Models;
using dominikz.dev.Utils;
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

    public async Task<List<MediaPreviewVM>> GetPreview(CancellationToken cancellationToken = default)
    {
        var vmList = await _client.Get<MediaPreviewVM>($"{Endpoint}/preview", cancellationToken);
        foreach (var vm in vmList)
            AttachApiKey(vm, _options);

        return vmList;
    }

    public static void AttachApiKey(MediaVM? vm, IOptions<ApiOptions> options)
    {
        if (string.IsNullOrWhiteSpace(vm?.Image?.Url))
            return;

        vm.Image.Url = QueryUtils.AttachParam(vm.Image.Url, ApiClient.ApiKeyHeaderName, options.Value.Key);
    }
}