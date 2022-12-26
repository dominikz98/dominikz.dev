using dominikz.shared.Filter;
using dominikz.shared.ViewModels;

namespace dominikz.dev.Endpoints;

public class MovieEndpoints
{
    private readonly ApiClient _client;
    private const string Endpoint = "medias/movies";

    public MovieEndpoints(ApiClient client)
    {
        _client = client;
    }

    public async Task<MovieDetailVM?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await _client.Get<MovieDetailVM>($"{Endpoint}", id, cancellationToken);

    public async Task<List<MovieVM>> Search(MoviesFilter filter, CancellationToken cancellationToken = default)
        => await _client.Get<MovieVM>($"{Endpoint}/search", filter, cancellationToken);
    
    public string CurlSearch(MoviesFilter filter)
        =>  _client.Curl($"{Endpoint}/search", filter);
}
