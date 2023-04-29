using dominikz.Domain.Enums;
using dominikz.Domain.Enums.Movies;
using dominikz.Domain.Options;
using dominikz.Domain.Structs;
using dominikz.Domain.ViewModels.Movies;
using Microsoft.Extensions.Options;

namespace dominikz.Client.Api;

public class DownloadEndpoints
{
    private readonly ApiClient _client;
    private readonly IOptions<ApiOptions> _options;
    private const string Endpoint = "download";

    public DownloadEndpoints(ApiClient client, IOptions<ApiOptions> options)
    {
        _client = client;
        _options = options;
    }

    public async Task<FileStruct?> Image(Guid imageId, ImageSizeEnum size, CancellationToken cancellationToken = default)
        => await _client.Download($"{Endpoint}/image/{imageId}/{size}", imageId.ToString(), cancellationToken);

    public async Task<FileStruct?> RawImage(string url, CancellationToken cancellationToken = default)
    {
        var client = new HttpClient();
        var response = await client.GetAsync(url, HttpCompletionOption.ResponseContentRead, cancellationToken);
        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var contentType = response.Content.Headers.ContentType?.ToString();
        if (contentType is null)
            return null;

        return new FileStruct(Guid.NewGuid().ToString(), contentType, stream);
    }

    public async Task<StreamTokenVm?> CreateMovieStreamingToken(Guid movieId, CancellationToken cancellationToken = default)
        => await CreateStreamingToken(StreamTokenPrefix.Movie, movieId, cancellationToken);

    public async Task<StreamTokenVm?> CreateTrailerStreamingToken(Guid movieId, CancellationToken cancellationToken = default)
        => await CreateStreamingToken(StreamTokenPrefix.Trailer, movieId, cancellationToken);

    private async Task<StreamTokenVm?> CreateStreamingToken(StreamTokenPrefix prefix, Guid id, CancellationToken cancellationToken = default)
        => await _client.Post<CreateStreamTokenVm, StreamTokenVm>($"{Endpoint}/stream/token", new() { Id = id, Prefix = prefix }, false, cancellationToken);


    public string CreateMovieStreamingUrl(Guid movieId, string token)
        => CreateStreamingUrl(StreamTokenPrefix.Movie, movieId, token);

    public string CreateTrailerStreamingUrl(Guid movieId, string token)
        => CreateStreamingUrl(StreamTokenPrefix.Trailer, movieId, token);

    private string CreateStreamingUrl(StreamTokenPrefix prefix, Guid id, string token)
        => $"{_options.Value.ApiUrl}/{ApiClient.Prefix}/{Endpoint}/stream/{(int)prefix}/{id}?{ApiClient.ApiKeyHeaderName}={_options.Value.ApiKey}&token={token}";
}