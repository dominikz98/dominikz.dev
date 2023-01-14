using System.ServiceModel.Syndication;
using System.Xml;
using dominikz.Domain.Enums;
using dominikz.Domain.Enums.Blog;
using dominikz.Domain.Options;
using dominikz.Domain.ViewModels.Blog;
using dominikz.Infrastructure.Mapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace dominikz.Infrastructure.Clients;

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

    public async Task<IReadOnlyCollection<ArticleVm>> GetArticlesByCategory(ArticleCategoryEnum? category, bool suppressCache)
    {
        if (suppressCache || _cache.TryGetValue<List<ArticleVm>>(CacheKey, out var articles) == false)
        {
            articles = await GetAllArticles();
            _cache.Set(CacheKey, articles, DateTimeOffset.UtcNow.AddHours(_options.Value.CacheDurationInH));
        }
        
        return category is null 
            ? articles! 
            : articles!.Where(x => x.Category == category).ToList();
    }

    private Task<List<ArticleVm>> GetAllArticles()
    {
        var medlanArticles = ParseRssFeed($"{_options.Value.Url}?feed=rss2");
        var projectMedlanArticles = ParseRssFeed($"{_options.Value.ProjectUrl}?feed=rss2");
        var articles = medlanArticles.Union(projectMedlanArticles)
            .MapToVm()
            .ToList();

        return Task.FromResult(articles);
    }
    
    private List<SyndicationItem> ParseRssFeed(string url)
    {
        using var reader = XmlReader.Create(url, new XmlReaderSettings() { Async = true });
        return SyndicationFeed.Load(reader).Items.ToList();
    }
}