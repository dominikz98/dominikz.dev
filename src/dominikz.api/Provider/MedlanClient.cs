using System.ServiceModel.Syndication;
using System.Xml;
using dominikz.api.Mapper;
using dominikz.api.Models.Options;
using dominikz.shared.ViewModels;
using Microsoft.Extensions.Options;

namespace dominikz.api.Provider;

public class MedlanClient
{
    private readonly IOptions<ExternalUrls> _options;

    public MedlanClient(IOptions<ExternalUrls> options)
    {
        _options = options;
    }

    public IReadOnlyCollection<ArticleListVm> GetArticlesByCategory()
    {
        var medlanArticles = ParseRssFeed($"{_options.Value.Medlan}?feed=rss2");
        var projectMedlanArticles = ParseRssFeed($"{_options.Value.ProjectMedlan}?feed=rss2");
        return medlanArticles.Union(projectMedlanArticles).MapToVm().ToList();
    }

    private IReadOnlyCollection<SyndicationItem> ParseRssFeed(string url)
    {
        using var reader = XmlReader.Create(url, new XmlReaderSettings() { Async = true });
        return SyndicationFeed.Load(reader).Items.ToList();
    }
}