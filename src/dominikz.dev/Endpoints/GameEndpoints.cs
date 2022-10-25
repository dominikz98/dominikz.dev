using dominikz.kernel.Filter;
using dominikz.kernel.ViewModels;

namespace dominikz.dev.Endpoints;

public class GameEndpoints
{
    private readonly ApiClient _client;
    private static readonly string _endpoint = "medias/games";

    public GameEndpoints(ApiClient client)
    {
        _client = client;
    }

    public async Task<List<GameVM>> Search(GamesFilter filter, CancellationToken cancellationToken = default)
        => await _client.Get<GameVM>($"{_endpoint}/search", filter, cancellationToken);
}
