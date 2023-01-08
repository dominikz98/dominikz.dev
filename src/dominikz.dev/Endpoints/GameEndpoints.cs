using dominikz.dev.Extensions;
using dominikz.dev.Models;
using dominikz.shared.Filter;
using dominikz.shared.ViewModels.Media;
using Microsoft.Extensions.Options;

namespace dominikz.dev.Endpoints;

public class GameEndpoints
{
    private readonly ApiClient _client;
    private readonly IOptions<ApiOptions> _options;
    private const string Endpoint = "medias/games";

    public GameEndpoints(ApiClient client, IOptions<ApiOptions> options)
    {
        _client = client;
        _options = options;
    }

    public async Task<List<GameVm>> Search(GamesFilter filter, CancellationToken cancellationToken = default)
    {
        var vmList = await _client.Get<GameVm>($"{Endpoint}/search", filter, cancellationToken);
        vmList.AttachApiKey(_options.Value.Key);
        return vmList;
    }

    public string CurlSearch(GamesFilter filter)
        =>  _client.Curl($"{Endpoint}/search", filter);
}
