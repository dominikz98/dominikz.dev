using dominikz.shared.Filter;
using dominikz.shared.ViewModels;

namespace dominikz.dev.Endpoints;

public class BlogEndpoints
{
    private readonly ApiClient _client;
    private static readonly string _endpoint = "blog";

    public BlogEndpoints(ApiClient client)
    {
        _client = client;
    }

    public async Task<ArticleDetailVm?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await _client.Get<ArticleDetailVm>(_endpoint, id, cancellationToken);

    public async Task<List<ArticleListVm>> Search(ArticleFilter filter, CancellationToken cancellationToken = default)
        => await _client.Get<ArticleListVm>($"{_endpoint}/search", filter, cancellationToken);
}