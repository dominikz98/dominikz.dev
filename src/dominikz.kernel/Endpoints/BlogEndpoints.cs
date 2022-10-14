using dominikz.kernel.ViewModels;

namespace dominikz.kernel.Endpoints;

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

public class ArticleFilter : IFilter
{
    public Guid? Id { get; set; }
    public string? Text { get; set; }
    public ArticleCategoryEnum Category { get; set; }

    public static ArticleFilter Default => new();

    public IReadOnlyCollection<FilterParam> GetParameter()
    {
        var result = new List<FilterParam>();

        if (Id is not null)
            result.Add(new(nameof(Id), Id.ToString()!));

        if (Text is not null)
            result.Add(new(nameof(Text), Text));

        if (Category != ArticleCategoryEnum.ALL)
            result.Add(new(nameof(Category), Category.ToString()!));

        return result;
    }
}
