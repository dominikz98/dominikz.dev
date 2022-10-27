using dominikz.kernel.Contracts;

namespace dominikz.api.Provider;

public class NoobitClient
{
    private readonly HttpClient _client;

    private readonly string _api = "api";

    public NoobitClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<List<NoobitArticleVM>> GetCodingArticles(CancellationToken cancellationToken)
        => await GetArticleByCategory(ArticleCategoryEnum.Coding, cancellationToken);

    public async Task<List<NoobitArticleVM>> GetBirdsArticles(CancellationToken cancellationToken)
        => await GetArticleByCategory(ArticleCategoryEnum.Birds, cancellationToken);

    public async Task<List<NoobitArticleVM>> GetThoughtsArticles(CancellationToken cancellationToken)
        => await GetArticleByCategory(ArticleCategoryEnum.Thoughts, cancellationToken);

    public async Task<List<NoobitArticleVM>> GetTravelArticles(CancellationToken cancellationToken)
        => await GetArticleByCategory(ArticleCategoryEnum.Travel, cancellationToken);

    public async Task<List<NoobitArticleVM>> GetAllArticles(CancellationToken cancellationToken)
    {
        var codingArticles = await GetArticleByCategory(ArticleCategoryEnum.Coding, cancellationToken);
        var travelArticles = await GetArticleByCategory(ArticleCategoryEnum.Travel, cancellationToken);
        var birdsArticles = await GetArticleByCategory(ArticleCategoryEnum.Birds, cancellationToken);
        var thoughtsArticles = await GetArticleByCategory(ArticleCategoryEnum.Thoughts, cancellationToken);

        return codingArticles.Union(travelArticles)
            .Union(birdsArticles)
            .Union(thoughtsArticles)
            .ToList();
    }

    private async Task<List<NoobitArticleVM>> GetArticleByCategory(ArticleCategoryEnum category, CancellationToken cancellationToken)
    {
        var vms = await _client.GetFromJsonAsync<List<NoobitArticleVM>>($"{_api}/blog/{category}/overview", cancellationToken) ?? new List<NoobitArticleVM>();

        // attach article url
        foreach (var vm in vms)
        {
            vm.Url = $"{_client.BaseAddress}blog/{category}/{vm.Topic.SeoName}/{vm.SeoTitle}".ToLower();
            vm.ImageUrl = $"{_client.BaseAddress}assets/blog/{category}/index.webp".ToLower();
        }

        return vms;
    }
}

public class NoobitArticleVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public NoobitTopicVM Topic { get; set; } = new();
    public string SeoTitle { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}

public class NoobitTopicVM
{
    public string Name { get; set; } = string.Empty;
    public string SeoName { get; set; } = string.Empty;
    public NoobitBlogVM Blog { get; set; } = new();
}

public class NoobitBlogVM
{
    public string SeoName { get; set; } = string.Empty;
}