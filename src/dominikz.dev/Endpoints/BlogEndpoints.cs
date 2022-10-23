using dominikz.kernel.Filter;
using dominikz.kernel.ViewModels;

namespace dominikz.dev.Endpoints;

public class BlogEndpoints
{
    private readonly ApiClient _client;
    private static readonly string _endpoint = "blog";

    public BlogEndpoints(ApiClient client)
    {
        _client = client;
    }

    public async Task<ArticleDetailVM?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await _client.Get<ArticleDetailVM>(_endpoint, id, cancellationToken);

    public async Task<List<ArticleListVM>> Search(ArticleFilter filter, CancellationToken cancellationToken = default)
        => await _client.Get<ArticleListVM>($"{_endpoint}/search", filter, cancellationToken);
}