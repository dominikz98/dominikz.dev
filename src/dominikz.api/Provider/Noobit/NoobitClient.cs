using dominikz.api.Mapper;
using dominikz.api.Models.Options;
using dominikz.shared.Enums;
using dominikz.shared.ViewModels.Blog;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace dominikz.api.Provider.Noobit;

public class NoobitClient
{
    private readonly HttpClient _client;
    private readonly IMemoryCache _cache;
    private readonly IOptions<NoobitOptions> _options;

    private const string CacheKey = "Noobit_Articles";
    private const string Api = "api";

    public NoobitClient(HttpClient client, IMemoryCache cache, IOptions<NoobitOptions> options)
    {
        _client = client;
        _cache = cache;
        _options = options;
    }

    public async Task<List<ArticleVm>> GetArticlesByCategory(ArticleCategoryEnum? category, CancellationToken cancellationToken)
    {
        var articles = await _cache.GetOrCreateAsync<List<ArticleVm>>(CacheKey, async options =>
        {
            options.AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(_options.Value.CacheDurationInH);
            
            var codingArticles = await GetArticleByCategory(ArticleCategoryEnum.Coding, cancellationToken);
            var travelArticles = await GetArticleByCategory(ArticleCategoryEnum.Travel, cancellationToken);
            var birdsArticles = await GetArticleByCategory(ArticleCategoryEnum.Birds, cancellationToken);
            var thoughtsArticles = await GetArticleByCategory(ArticleCategoryEnum.Thoughts, cancellationToken);
            
            return codingArticles.Union(travelArticles)
                .Union(birdsArticles)
                .Union(thoughtsArticles)
                .ToList();
        }) ?? new List<ArticleVm>();
        
        if (category is null)
            return articles;
        
        return articles.Where(x => x.Category == category).ToList();
    }

    private async Task<List<ArticleVm>> GetArticleByCategory(ArticleCategoryEnum category, CancellationToken cancellationToken)
    {
        var vms = await _client.GetFromJsonAsync<List<NoobitArticleVm>>($"{Api}/blog/{category}/overview", cancellationToken) ?? new List<NoobitArticleVm>();

        // attach article url
        foreach (var vm in vms)
        {
            vm.Url = $"{_client.BaseAddress}blog/{category}/{vm.Topic.SeoName}/{vm.SeoTitle}".ToLower();
            vm.ImageUrl = $"{_client.BaseAddress}assets/blog/{category}/index.webp".ToLower();
        }

        return vms.MapToVm().ToList();
    }
}