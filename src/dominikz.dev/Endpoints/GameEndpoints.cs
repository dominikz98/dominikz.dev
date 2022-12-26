using dominikz.shared.Filter;
using dominikz.shared.ViewModels;

namespace dominikz.dev.Endpoints;

public class GameEndpoints
{
    private readonly ApiClient _client;
    private const string Endpoint = "medias/games";

    public GameEndpoints(ApiClient client)
    {
        _client = client;
    }

    public async Task<List<GameVM>> Search(GamesFilter filter, CancellationToken cancellationToken = default)
        => await _client.Get<GameVM>($"{Endpoint}/search", filter, cancellationToken);
    
    public string CurlSearch(GamesFilter filter)
        =>  _client.Curl($"{Endpoint}/search", filter);
}
