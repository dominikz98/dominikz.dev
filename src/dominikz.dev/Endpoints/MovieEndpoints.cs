using dominikz.dev.Extensions;
using dominikz.dev.Models;
using dominikz.shared.Filter;
using dominikz.shared.ViewModels.Media;
using Microsoft.Extensions.Options;

namespace dominikz.dev.Endpoints;

public class MovieEndpoints
{
    private readonly ApiClient _client;
    private readonly IOptions<ApiOptions> _options;
    private const string Endpoint = "medias/movies";

    public MovieEndpoints(ApiClient client, IOptions<ApiOptions> options)
    {
        _client = client;
        _options = options;
    }

    public async Task<MovieDetailVM?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var vm = await _client.GetSingle<MovieDetailVM>($"{Endpoint}/{id}", cancellationToken);
        if (vm is null)
            return null;

        // attach image api keys
        vm.AttachApiKey(_options.Value.Key);
        vm.Author?.AttachApiKey(_options.Value.Key);
        vm.Writers.AttachApiKey(_options.Value.Key);
        vm.Stars.AttachApiKey(_options.Value.Key);
        vm.Directors.AttachApiKey(_options.Value.Key);

        return vm;
    }

    public async Task<List<MovieVM>> Search(MoviesFilter filter, CancellationToken cancellationToken = default)
    {
        var vmList = await _client.Get<MovieVM>($"{Endpoint}/search", filter, cancellationToken);
        vmList.AttachApiKey(_options.Value.Key);
        return vmList;
    }

    public string CurlSearch(MoviesFilter filter)
        => _client.Curl($"{Endpoint}/search", filter);
}