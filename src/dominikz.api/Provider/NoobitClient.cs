using dominikz.api.Mapper;
using dominikz.api.Models.ViewModels;
using dominikz.shared.Contracts;
using dominikz.shared.ViewModels;

namespace dominikz.api.Provider;

public class NoobitClient
{
    private readonly HttpClient _client;

    private readonly string _api = "api";

    public NoobitClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<List<ArticleListVm>> GetArticlesByCategory(ArticleCategoryEnum category, CancellationToken cancellationToken)
    {
        if (category == ArticleCategoryEnum.ALL)
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

        return await GetArticleByCategory(category, cancellationToken);
    }

    private async Task<List<ArticleListVm>> GetArticleByCategory(ArticleCategoryEnum category, CancellationToken cancellationToken)
    {
        var vms = await _client.GetFromJsonAsync<List<NoobitArticleVm>>($"{_api}/blog/{category}/overview", cancellationToken) ?? new List<NoobitArticleVm>();

        // attach article url
        foreach (var vm in vms)
        {
            vm.Url = $"{_client.BaseAddress}blog/{category}/{vm.Topic.SeoName}/{vm.SeoTitle}".ToLower();
            vm.ImageUrl = $"{_client.BaseAddress}assets/blog/{category}/index.webp".ToLower();
        }

        return vms.MapToVm().ToList();
    }
}