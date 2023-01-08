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

    public async Task<MovieDetailVm?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var vm = await _client.GetSingle<MovieDetailVm>($"{Endpoint}/{id}", cancellationToken);
        if (vm is null)
            return null;

        // attach image api keys
        vm.AttachApiKey(_options.Value.Key);
        vm.AuthorImageUrl = ViewModelExtensions.AttachApiKey(vm.AuthorImageUrl, _options.Value.Key);
        vm.Writers.AttachApiKey(_options.Value.Key);
        vm.Stars.AttachApiKey(_options.Value.Key);
        vm.Directors.AttachApiKey(_options.Value.Key);

        return vm;
    }

    public async Task<EditMovieWrapper?> GetDraftById(Guid id, CancellationToken cancellationToken = default)
    {
        var movie = await _client.GetSingle<EditMovieWrapper?>($"{Endpoint}/draft/{id}", cancellationToken);
        if (movie == null)
            return null;

        movie.DirectorsWrappers = movie.Directors.Select(x => x.Wrap()).ToList();
        movie.WritersWrappers = movie.Writers.Select(x => x.Wrap()).ToList();
        movie.StarsWrappers = movie.Stars.Select(x => x.Wrap()).ToList();
        return movie;
    }

    public async Task<MovieTemplateVm?> GetTemplateByImdbId(string imdbId, CancellationToken cancellationToken = default)
        => await _client.GetSingle<MovieTemplateVm>($"{Endpoint}/template/{imdbId}", cancellationToken);

    public async Task<List<MovieVm>> Search(MoviesFilter filter, CancellationToken cancellationToken = default)
    {
        var vmList = await _client.Get<MovieVm>($"{Endpoint}/search", filter, cancellationToken);
        vmList.AttachApiKey(_options.Value.Key);
        return vmList;
    }

    public async Task<MovieDetailVm?> Add(EditMovieWrapper vm, CancellationToken cancellationToken = default)
    {
        var images = vm.Image.Union(vm.DirectorsWrappers.SelectMany(x => x.Image))
            .Union(vm.WritersWrappers.SelectMany(x => x.Image))
            .Union(vm.StarsWrappers.SelectMany(x => x.Image))
            .ToList();

        return await _client.Upload<EditMovieVm, MovieDetailVm>(HttpMethod.Post, Endpoint, vm, images, cancellationToken);
    }

    public async Task<MovieDetailVm?> Update(EditMovieWrapper vm, CancellationToken cancellationToken = default)
    {
        var images = vm.Image.Union(vm.DirectorsWrappers.SelectMany(x => x.Image))
            .Union(vm.WritersWrappers.SelectMany(x => x.Image))
            .Union(vm.StarsWrappers.SelectMany(x => x.Image))
            .ToList();

        return await _client.Upload<EditMovieVm, MovieDetailVm>(HttpMethod.Put, Endpoint, vm, images, cancellationToken);
    }

    public string CurlSearch(MoviesFilter filter)
        => _client.Curl($"{Endpoint}/search", filter);
}