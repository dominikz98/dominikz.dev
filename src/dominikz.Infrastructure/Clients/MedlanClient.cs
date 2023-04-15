using System.ServiceModel.Syndication;
using System.Xml;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using dominikz.Domain.Enums.Blog;
using dominikz.Domain.Models;
using dominikz.Domain.Options;
using Microsoft.Extensions.Options;

namespace dominikz.Infrastructure.Clients;

public class MedlanClient
{
    private readonly IOptions<MedlanOptions> _options;
    private static readonly Guid DefaultImageId = Guid.Parse("6887b330-7e26-11ed-8afc-ccf04caa7138");

    public MedlanClient(IOptions<MedlanOptions> options)
    {
        _options = options;
    }

    public async Task<IReadOnlyCollection<ExtArticleShadow>> GetArticles(CancellationToken cancellationToken)
    {
        using var reader = XmlReader.Create($"{_options.Value.Url}?feed=rss2", new XmlReaderSettings()
        {
            Async = true,
            DtdProcessing = DtdProcessing.Parse
        });
        var rssItems = SyndicationFeed.Load(reader).Items.ToList();
        var result = new List<ExtArticleShadow>();
        foreach (var item in rssItems)
        {
            var article = await GetArticleFromUrl(item.Links.First().Uri.ToString(), cancellationToken);
            result.Add(article);
        }

        return result;
    }

    private async Task<ExtArticleShadow> GetArticleFromUrl(string url, CancellationToken cancellationToken)
    {
        var html = await new HttpClient().GetStringAsync(url, cancellationToken);
        var document = await new HtmlParser().ParseDocumentAsync(html);
        var excludeThumbnailUrls = new[]
        {
            "https://blogcontent.medlan.de/blog_img/KeinCover.png",
            "https://www.medlan.de/wp-content/plugins/complianz-gdpr/assets/images/placeholders/default-minimal.jpg"
        };

        var title = document.QuerySelector(".entry-title")?.TextContent.Trim() ?? throw new InvalidCastException($"Cant parse title! ({url})");

        var date = DateTime.TryParse(document.QuerySelector(".entry-date.published")
            ?.GetAttribute("datetime"), out var parsedDate)
            ? parsedDate
            : throw new InvalidCastException($"Cant parse date! ({url})");

        var categories = document.QuerySelector(".entry-meta-term-single")
            ?.QuerySelectorAll("a")
            .Select(x => x.TextContent.Trim())
            .ToList() ?? throw new InvalidCastException($"Cant parse category! ({url})");

        var thumbnailImages = document.QuerySelector("article")?.QuerySelectorAll("img").ToList() ?? new List<IElement>();
        var thumbnailSrc = thumbnailImages.Select(x => x.GetAttribute("data-src-cmplz"))
            .Union(thumbnailImages.Select(x => x.GetAttribute("src")))
            .Where(x => string.IsNullOrWhiteSpace(x) == false)
            .Where(x => !excludeThumbnailUrls.Contains(x))
            .FirstOrDefault();

        var shadow = new ExtArticleShadow()
        {
            Title = title,
            Date = DateOnly.FromDateTime(date),
            Url = url,
            Source = ArticleSourceEnum.Medlan,
            Category = TryAssignSystemCategories(categories) ?? TryAssignSystemCategoriesViaName(title) ?? ArticleCategoryEnum.Unknown
        };

        return await AssignImage(shadow, thumbnailSrc, cancellationToken);
    }

    private static async Task<ExtArticleShadow> AssignImage(ExtArticleShadow shadow, string? imageUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            shadow.ImageId = DefaultImageId;
            return shadow;
        }

        try
        {
            shadow.Image = await new HttpClient().GetStreamAsync(imageUrl, cancellationToken);
            shadow.ImageId = Guid.NewGuid();
        }
        catch (Exception)
        {
            shadow.ImageId = DefaultImageId;
        }

        return shadow;
    }

    private static readonly List<string> MusicAssignments = new()
    {
        "Musik", "Konzerte", "Konzert", "Acoustic Adventures", "Neue CDs", "Bands", "CD-Manager", "MusicBrainz", "MP3 Player", "Neuerscheinungen", "CD Review", "Summer Breeze", "CD", "CD-Regal", "CD-Sammlung", "CD-Schrank", "CD-Statistik", "Festival"
    };

    private static readonly List<string> TravelAssignments = new()
    {
        "Reisen", "Urlaub", "Freizeitpark", "Europa Park"
    };

    private static readonly List<string> MovieAssignments = new()
    {
        "Filme", "Videothek", "Kino", "DVD Verleih", "Lovefilm", "Cinemaxx"
    };

    private static readonly List<string> GamingAssignments = new()
    {
        "Spiele", "Steam"
    };

    private static readonly List<string> CodingAssignments = new()
    {
        "Programmierung", "Angular", "CSS", "HTML", "Javascript", "MySQL", "Typescript", "Github", "NetCore", "Visual Studio", "C#", "Records", "Tutorial", "NodeJS", "NPM", "jQuery", "Socket.io", "FreeDB", "Code Snipptes", "Wordpress", "Arduino", "Project Medlan", "Software", "Digital"
    };

    private static readonly List<string> FinanceAssignments = new()
    {
        "Aktie", "Geld", "Dividende"
    };

    private static readonly List<string> ShoppingAssignments = new()
    {
        "Schäppchen", "Whiskey", "Aktion", "Angebot"
    };


    private static ArticleCategoryEnum? TryAssignSystemCategories(IReadOnlyCollection<string> categories)
    {
        ArticleCategoryEnum? sysCategory = null;

        // music
        var musicCategoryFound = categories.Any(x => MusicAssignments.Any(y => y.Equals(x, StringComparison.OrdinalIgnoreCase)));
        if (musicCategoryFound)
            sysCategory = ArticleCategoryEnum.Music;

        // movies
        var movieCategoryFound = categories.Any(x => MovieAssignments.Any(y => y.Equals(x, StringComparison.OrdinalIgnoreCase)));
        if (sysCategory is null && movieCategoryFound)
            sysCategory = ArticleCategoryEnum.Movie;

        // travel
        var travelCategoryFound = categories.Any(x => TravelAssignments.Any(y => y.Equals(x, StringComparison.OrdinalIgnoreCase)));
        if (sysCategory is null && travelCategoryFound)
            sysCategory = ArticleCategoryEnum.Travel;

        // gaming
        var gamingCategoryFound = categories.Any(x => GamingAssignments.Any(y => y.Equals(x, StringComparison.OrdinalIgnoreCase)));
        if (sysCategory is null && gamingCategoryFound)
            sysCategory = ArticleCategoryEnum.Gaming;

        // coding
        var codingCategoryFound = categories.Any(x => CodingAssignments.Any(y => y.Equals(x, StringComparison.OrdinalIgnoreCase)));
        if (sysCategory is null && codingCategoryFound)
            sysCategory = ArticleCategoryEnum.Coding;

        // finance
        var financeCategoryFound = categories.Any(x => FinanceAssignments.Any(y => y.Equals(x, StringComparison.OrdinalIgnoreCase)));
        if (sysCategory is null && financeCategoryFound)
            sysCategory = ArticleCategoryEnum.Finance;

        // shopping
        var shoppingCategoryFound = categories.Any(x => ShoppingAssignments.Any(y => y.Equals(x, StringComparison.OrdinalIgnoreCase)));
        if (sysCategory is null && shoppingCategoryFound)
            sysCategory = ArticleCategoryEnum.Shopping;

        return sysCategory;
    }

    private static ArticleCategoryEnum? TryAssignSystemCategoriesViaName(string title)
    {
        ArticleCategoryEnum? sysCategory = null;

        // music
        var musicCategoryFound = MusicAssignments.Any(y => title.Contains(y, StringComparison.OrdinalIgnoreCase));
        if (musicCategoryFound)
            sysCategory = ArticleCategoryEnum.Music;

        // movies
        var movieCategoryFound = MovieAssignments.Any(y => title.Contains(y, StringComparison.OrdinalIgnoreCase));
        if (sysCategory is null && movieCategoryFound)
            sysCategory = ArticleCategoryEnum.Movie;

        // travel
        var travelCategoryFound = TravelAssignments.Any(y => title.Contains(y, StringComparison.OrdinalIgnoreCase));
        if (sysCategory is null && travelCategoryFound)
            sysCategory = ArticleCategoryEnum.Travel;

        // gaming
        var gamingCategoryFound = GamingAssignments.Any(y => title.Contains(y, StringComparison.OrdinalIgnoreCase));
        if (sysCategory is null && gamingCategoryFound)
            sysCategory = ArticleCategoryEnum.Gaming;

        // coding
        var codingCategoryFound = CodingAssignments.Any(y => title.Contains(y, StringComparison.OrdinalIgnoreCase));
        if (sysCategory is null && codingCategoryFound)
            sysCategory = ArticleCategoryEnum.Coding;

        // finance
        var financeCategoryFound = FinanceAssignments.Any(y => title.Contains(y, StringComparison.OrdinalIgnoreCase));
        if (sysCategory is null && financeCategoryFound)
            sysCategory = ArticleCategoryEnum.Coding;

        // shopping
        var shoppingCategoryFound = ShoppingAssignments.Any(y => title.Contains(y, StringComparison.OrdinalIgnoreCase));
        if (sysCategory is null && shoppingCategoryFound)
            sysCategory = ArticleCategoryEnum.Coding;

        return sysCategory;
    }
}