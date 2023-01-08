using System.ServiceModel.Syndication;
using System.Xml;
using dominikz.api.Mapper;
using dominikz.api.Models.Options;
using dominikz.shared.Enums;
using dominikz.shared.ViewModels.Blog;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace dominikz.api.Provider;

public class MedlanClient
{
    private readonly IOptions<MedlanOptions> _options;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "Medlan_Project_Articles";

    public MedlanClient(IOptions<MedlanOptions> options, IMemoryCache cache)
    {
        _options = options;
        _cache = cache;
    }

    public async Task<IReadOnlyCollection<ArticleVm>> GetArticlesByCategory(ArticleCategoryEnum? category)
    {
        var articles = await _cache.GetOrCreateAsync<List<ArticleVm>>(CacheKey, options =>
        {
            options.AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(_options.Value.CacheDurationInH);
            var medlanArticles = ParseRssFeed($"{_options.Value.Url}?feed=rss2");
            var projectMedlanArticles = ParseRssFeed($"{_options.Value.ProjectUrl}?feed=rss2");
            var articles = medlanArticles.Union(projectMedlanArticles)
                .MapToVm()
                .ToList();

            return Task.FromResult(articles);
        }) ?? new List<ArticleVm>();

        if (category is null)
            return articles;
        
        return articles.Where(x => x.Category == category).ToList();
    }

    private IReadOnlyCollection<SyndicationItem> ParseRssFeed(string url)
    {
        using var reader = XmlReader.Create(url, new XmlReaderSettings() { Async = true });
        return SyndicationFeed.Load(reader).Items.ToList();
    }
}