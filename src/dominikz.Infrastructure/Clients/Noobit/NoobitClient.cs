using System.Net.Http.Json;
using dominikz.Domain.Enums.Blog;
using dominikz.Domain.Models;

namespace dominikz.Infrastructure.Clients.Noobit;

public class NoobitClient
{
    private readonly HttpClient _client;
    private const string Api = "api";

    public NoobitClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyCollection<ExtArticleShadow>> GetArticles(CancellationToken cancellationToken)
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

    private async Task<IReadOnlyCollection<ExtArticleShadow>> GetArticleByCategory(ArticleCategoryEnum category, CancellationToken cancellationToken)
    {
        var vms = await _client.GetFromJsonAsync<List<NoobitArticleVm>>($"{Api}/blog/{category}/overview", cancellationToken) ?? new List<NoobitArticleVm>();

        // attach article url
        foreach (var vm in vms)
        {
            vm.Url = $"{_client.BaseAddress}blog/{category}/{vm.Topic.SeoName}/{vm.SeoTitle}".ToLower();
            vm.ImageUrl = $"{_client.BaseAddress}assets/blog/{category}/index.webp".ToLower();
        }

        return vms.Select(x => new ExtArticleShadow()
        {
            Title = x.Title,
            Date = DateOnly.FromDateTime(x.Date),
            Category = Enum.Parse<ArticleCategoryEnum>(x.Topic.Blog.SeoName, true),
            Url = x.Url,
            ImageUrl = x.ImageUrl,
            Source = ArticleSourceEnum.Noobit
        }).ToList();
    }
}