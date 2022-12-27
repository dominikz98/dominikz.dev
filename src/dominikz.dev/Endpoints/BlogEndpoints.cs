using dominikz.dev.Models;
using dominikz.shared.Filter;
using dominikz.shared.ViewModels;
using Microsoft.Extensions.Options;

namespace dominikz.dev.Endpoints;

public class BlogEndpoints
{
    private readonly ApiClient _client;
    private readonly IOptions<ExternalUrls> _options;
    private const string Endpoint = "blog";

    public BlogEndpoints(ApiClient client, IOptions<ExternalUrls> options)
    {
        _client = client;
        _options = options;
    }
    
    public string GetRssFeedUrl()
        => $"{_options.Value.Api}{ApiClient.Prefix}/{Endpoint}/rss";
    
    public async Task<ArticleDetailVm?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await _client.GetSingle<ArticleDetailVm>($"{Endpoint}/{id}", cancellationToken);

    public async Task<List<ArticleListVm>> Search(ArticleFilter filter, CancellationToken cancellationToken = default)
        => await _client.Get<ArticleListVm>($"{Endpoint}/search", filter, cancellationToken);

    public string CurlSearch(ArticleFilter filter)
        => _client.Curl($"{Endpoint}/search", filter);
}