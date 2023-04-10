using dominikz.Domain.Filter;
using dominikz.Domain.Options;
using dominikz.Domain.Structs;
using dominikz.Domain.ViewModels;
using dominikz.Domain.ViewModels.Media;
using dominikz.Infrastructure.Extensions;
using Microsoft.Extensions.Options;

namespace dominikz.Infrastructure.Clients.Api;

public class MovieEndpoints
{
    private readonly ApiClient _client;
    private readonly IOptions<ApiOptions> _options;
    private const string Endpoint = "movies";

    public MovieEndpoints(ApiClient client, IOptions<ApiOptions> options)
    {
        _client = client;
        _options = options;
    }

    public async Task<MovieDetailVm?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var vm = await _client.GetSingle<MovieDetailVm>($"{Endpoint}/{id}", cancellationToken);
        if (vm is null)
            return null;

        // attach image api keys
        vm.AttachApiKey(_options.Value.Key);
        return vm;
    }

    public async Task<EditMovieVm?> GetDraftById(Guid id, CancellationToken cancellationToken = default)
        => await _client.GetSingle<EditMovieVm?>($"{Endpoint}/draft/{id}", cancellationToken);

    public async Task<MovieTemplateVm?> GetTemplateByImdbId(string imdbId, CancellationToken cancellationToken = default)
        => await _client.GetSingle<MovieTemplateVm>($"{Endpoint}/template/{imdbId}", cancellationToken);

    public async Task<List<MovieVm>> Search(MoviesFilter filter, CancellationToken cancellationToken = default)
    {
        var vmList = await _client.Get<MovieVm>($"{Endpoint}/search", filter, cancellationToken);
        vmList.AttachApiKey(_options.Value.Key);
        return vmList;
    }

    public async Task<MovieDetailVm?> Add(EditMovieVm vm, List<FileStruct> files, CancellationToken cancellationToken = default)
        => await _client.Upload<EditMovieVm, MovieDetailVm>(HttpMethod.Post, Endpoint, vm, files, cancellationToken);

    public async Task<MovieDetailVm?> Update(EditMovieVm vm, List<FileStruct> files, CancellationToken cancellationToken = default)
        => await _client.Upload<EditMovieVm, MovieDetailVm>(HttpMethod.Put, Endpoint, vm, files, cancellationToken);

    public async Task<StreamTokenVm?> CreateStreamingToken(Guid movieId, CancellationToken cancellationToken = default)
        => await _client.Post<EmptyVm, StreamTokenVm>($"{Endpoint}/{movieId}/stream/token", new(), false, cancellationToken);

    public string CreateStreamingUrl(Guid movieId, string token)
        => $"{ApiClient.Prefix}/{Endpoint}/{movieId}/stream?{ApiClient.ApiKeyHeaderName}={_options.Value.Key}&token={token}";

    public string CurlSearch(MoviesFilter filter)
        => _client.Curl($"{Endpoint}/search", filter);
}