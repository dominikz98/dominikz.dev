using System.ServiceModel.Syndication;
using dominikz.Domain.Enums.Blog;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Blog;
using dominikz.Infrastructure.Clients.Noobit;

namespace dominikz.Infrastructure.Mapper;

public static class ArticleMapper
{
    private static readonly Guid MarkusLieblDefaultImageId = Guid.Parse("6887b330-7e26-11ed-8afc-ccf04caa7138");

    private static readonly Dictionary<ArticleCategoryEnum, Guid> DefaultCategoryImageId = new()
    {
        { ArticleCategoryEnum.Coding, Guid.Parse("54236c67-7ec2-11ed-8ba0-cef86e40f50e") },
        { ArticleCategoryEnum.Music, Guid.Parse("cd1a43ff-7ec3-11ed-8ba0-cef86e40f50e") },
        { ArticleCategoryEnum.Movie, Guid.Parse("3f8cb91b-7ec4-11ed-8ba0-cef86e40f50e") },
        { ArticleCategoryEnum.Travel, Guid.Parse("ecfa8c15-7ef9-11ed-8cfa-d20ab77c47ec") },
        { ArticleCategoryEnum.Gaming, Guid.Parse("051562ac-7efa-11ed-8cfa-d20ab77c47ec") },
        { ArticleCategoryEnum.Project, Guid.Parse("11f6e4c8-7efa-11ed-8cfa-d20ab77c47ec") },
    };

    public static Article ApplyChanges(this Article original, EditArticleVm vm)
    {
        original.Id = vm.Id;
        original.Title = vm.Title;
        original.PublishDate = vm.PublishDate;
        original.Category = vm.Category;
        original.Tags = vm.Tags;
        original.HtmlText = vm.HtmlText;
        return original;
    }

    public static IQueryable<ArticleVm> MapToVm(this IQueryable<Article> query)
        => query.Select(article => new ArticleVm()
        {
            Id = article.Id,
            ImageUrl = article.Id.ToString(),
            Title = article.Title,
            PublishDate = article.PublishDate,
            Category = article.Category,
            Source = ArticleSourceEnum.Dz
        });

    public static IEnumerable<ArticleVm> MapToVm(this IEnumerable<NoobitArticleVm> query)
        => query.Select(article => new ArticleVm()
        {
            Id = article.Id,
            Title = article.Title,
            PublishDate = article.Date,
            Category = Enum.Parse<ArticleCategoryEnum>(article.Topic.Blog.SeoName, true),
            Path = article.Url,
            ImageUrl = article.ImageUrl,
            Source = ArticleSourceEnum.Noobit
        });

    public static IEnumerable<ArticleVm> MapToVm(this IEnumerable<SyndicationItem> query)
    {
        var result = new List<ArticleVm>();
        foreach (var item in query)
        {
            var sysCategory = TryAssignAndRemoveSystemCategories(item.Categories);
            if (sysCategory is null || DefaultCategoryImageId.TryGetValue(sysCategory.Value, out var imageId) == false)
                imageId = MarkusLieblDefaultImageId;

            var article = new ArticleVm()
            {
                Id = Guid.NewGuid(),
                Title = item.Title.Text,
                PublishDate = item.PublishDate.Date,
                Category = sysCategory ?? ArticleCategoryEnum.Unknown,
                AltCategories = string.Join(", ", item.Categories.Take(2).Select(x => x.Name)),
                Path = item.Links.FirstOrDefault()?.Uri.ToString() ?? string.Empty,
                ImageUrl = imageId.ToString(),
                Source = ArticleSourceEnum.Medlan
            };

            result.Add(article);
        }

        return result;
    }

    public static IQueryable<EditArticleVm> MapToEditVm(this IQueryable<Article> query)
        => query.Select(article => new EditArticleVm()
        {
            Id = article.Id,
            Category = article.Category,
            Tags = article.Tags,
            Title = article.Title,
            PublishDate = article.PublishDate,
            HtmlText = article.HtmlText
        });

    public static IQueryable<ArticleViewVm> MapToViewVm(this IQueryable<Article> query)
        => query.Select(article => article.MapToViewVm());

    private static ArticleViewVm MapToViewVm(this Article article)
        => new()
        {
            Id = article.Id,
            ImageUrl = article.Id.ToString(),
            Title = article.Title,
            PublishDate = article.PublishDate,
            Category = article.Category,
            Tags = article.Tags,
            Text = article.HtmlText,
            Source = ArticleSourceEnum.Dz
        };

    private static ArticleCategoryEnum? TryAssignAndRemoveSystemCategories(System.Collections.ObjectModel.Collection<SyndicationCategory> rssCategories)
    {
        ArticleCategoryEnum? sysCategory = null;

        // music
        var musicAssignments = new List<string>()
        {
            "Konzerte", "Acoustic Adventures", "Neue CDs", "Bands", "CD-Manager", "MusicBrainz", "MP3 Player", "Neuerscheinungen"
        };

        var musicCategoryFound = rssCategories.Any(x => musicAssignments.Any(y => y.Equals(x.Name, StringComparison.OrdinalIgnoreCase)));
        if (musicCategoryFound)
            sysCategory = ArticleCategoryEnum.Music;

        // movies
        var movieAssignments = new List<string>()
        {
            "Filme"
        };
        var movieCategoryFound = rssCategories.Any(x => movieAssignments.Any(y => y.Equals(x.Name, StringComparison.OrdinalIgnoreCase)));
        if (sysCategory is null && movieCategoryFound)
            sysCategory = ArticleCategoryEnum.Movie;

        // travel
        var travelAssignments = new List<string>()
        {
            "Reisen", "Urlaub"
        };
        var travelCategoryFound = rssCategories.Any(x => travelAssignments.Any(y => y.Equals(x.Name, StringComparison.OrdinalIgnoreCase)));
        if (sysCategory is null && travelCategoryFound)
            sysCategory = ArticleCategoryEnum.Travel;

        // coding
        var codingAssignments = new List<string>()
        {
            "Programmierung", "Angular", "CSS", "HTML", "Javascript", "MySQL", "Typescript", "Github", "NetCore", "Visual Studio", "C#", "Records", "Tutorial", "NodeJS", "NPM", "jQuery", "Socket.io", "FreeDB", "Code Snipptes", "Wordpress", "Arduino", "Project Medlan"
        };
        var codingCategoryFound = rssCategories.Any(x => codingAssignments.Any(y => y.Equals(x.Name, StringComparison.OrdinalIgnoreCase)));
        if (sysCategory is null && codingCategoryFound)
            sysCategory = ArticleCategoryEnum.Coding;

        // remove all system categories
        var allAssignments = musicAssignments.Union(movieAssignments)
            .Union(travelAssignments)
            .Union(codingAssignments)
            .ToList();

        var toRemoveList = rssCategories.Where(x => allAssignments.Any(y => y.Equals(x.Name, StringComparison.OrdinalIgnoreCase))).ToList();
        foreach (var toRemove in toRemoveList)
            rssCategories.Remove(toRemove);

        return sysCategory;
    }
}