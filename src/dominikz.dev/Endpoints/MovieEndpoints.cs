using dominikz.kernel.Filter;
using dominikz.kernel.ViewModels;

namespace dominikz.dev.Endpoints;

public class MovieEndpoints
{
    private readonly ApiClient _client;
    private static readonly string _endpoint = "movies";

    public MovieEndpoints(ApiClient client)
    {
        _client = client;
    }

    public async Task<MovieDetailVM?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await _client.Get<MovieDetailVM>($"{_endpoint}", id, cancellationToken);

    public async Task<List<MovieVM>> Search(MoviesFilter filter, CancellationToken cancellationToken = default)
        => await _client.Get<MovieVM>($"{_endpoint}/search", filter, cancellationToken);
}
