using System.Collections.ObjectModel;
using System.ServiceModel.Syndication;
using dominikz.api.Models;
using dominikz.api.Models.ViewModels;
using dominikz.shared.Contracts;
using dominikz.shared.ViewModels;

namespace dominikz.api.Mapper;

public static class ArticleMapper
{
    private static readonly Guid TobiasHaimerlId = Guid.Parse("c79efe4c-6b9e-4ce9-9e09-b85777082b32");
    private static readonly Guid MarkusLieblId = Guid.Parse("3fd2fb6d-7cb7-11ed-8bce-cffa786aaa15");
    
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

    public static IQueryable<ArticleListVm> MapToVm(this IQueryable<Article> query)
        => query.Select(article => new ArticleListVm()
        {
            Id = article.Id,
            Author = article.Author!.MapToVm(),
            Image = article.File!.MapToVm(),
            Title = article.Title,
            Timestamp = article.Timestamp,
            Category = article.Category,
            Source = ArticleSourceEnum.Dz
        });

    public static IEnumerable<ArticleListVm> MapToVm(this IEnumerable<NoobitArticleVm> query)
        => query.Select(article => new ArticleListVm()
        {
            Id = article.Id,
            Title = article.Title,
            Timestamp = article.Date,
            Category = Enum.Parse<ArticleCategoryEnum>(article.Topic.Blog.SeoName, true),
            Path = article.Url,
            Author = new PersonVM()
            {
                Id = TobiasHaimerlId,
                Name = "Tobias Haimerl",
                Image = new FileVM()
                {
                    Id = TobiasHaimerlId,
                    Category = FileCategoryEnum.IMAGE,
                    Extension = FileExtensionEnum.WEBP
                }
            },
            Image = new FileVM()
            {
                Id = article.Id,
                Category = FileCategoryEnum.IMAGE,
                Extension = FileExtensionEnum.WEBP,
                Url = article.ImageUrl
            },
            Source = ArticleSourceEnum.Noobit
        });

    public static IEnumerable<ArticleListVm> MapToVm(this IEnumerable<SyndicationItem> query)
    {
        var result = new List<ArticleListVm>();
        foreach (var item in query)
        {
            var sysCategory = TryAssignAndRemoveSystemCategories(item.Categories);
            if (sysCategory is null || DefaultCategoryImageId.TryGetValue(sysCategory.Value, out var imageId) == false)
                imageId = MarkusLieblDefaultImageId;

            var article = new ArticleListVm()
            {
                Id = Guid.NewGuid(),
                Title = item.Title.Text,
                Timestamp = item.PublishDate.Date,
                Category = sysCategory ?? ArticleCategoryEnum.Unknown,
                AltCategories = string.Join(", ", item.Categories.Take(2).Select(x => x.Name)),
                Path = item.Links.FirstOrDefault()?.Uri.ToString() ?? string.Empty,
                Author = new PersonVM()
                {
                    Id = MarkusLieblId,
                    Name = "Markus Liebl",
                    Image = new FileVM()
                    {
                        Id = MarkusLieblId,
                        Category = FileCategoryEnum.IMAGE,
                        Extension = FileExtensionEnum.WEBP
                    }
                },
                Image = new FileVM()
                {
                    Id = imageId,
                    Category = FileCategoryEnum.IMAGE,
                    Extension = FileExtensionEnum.JPG
                },
                Source = ArticleSourceEnum.Medlan
            };

            result.Add(article);
        }

        return result;
    }

    public static IQueryable<ArticleDetailVm> MapToDetailVm(this IQueryable<Article> query)
        => query.Select(article => new ArticleDetailVm()
        {
            Id = article.Id,
            Author = article.Author!.MapToVm(),
            Image = article.File!.MapToVm(),
            Title = article.Title,
            Timestamp = article.Timestamp,
            Category = article.Category,
            Tags = article.Tags,
            Text = article.MdText
        });

    private static ArticleCategoryEnum? TryAssignAndRemoveSystemCategories(Collection<SyndicationCategory> rssCategories)
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
        if (sysCategory  is null && travelCategoryFound)
            sysCategory = ArticleCategoryEnum.Travel;

        // coding
        var codingAssignments = new List<string>()
        {
            "Programmierung", "Angular", "CSS", "HTML", "Javascript", "MySQL", "Typescript", "Github", "NetCore", "Visual Studio", "C#", "Records", "Tutorial", "NodeJS", "NPM", "jQuery", "Socket.io", "FreeDB", "Code Snipptes", "Wordpress", "Arduino", "Project Medlan"
        };
        var codingCategoryFound = rssCategories.Any(x => codingAssignments.Any(y => y.Equals(x.Name, StringComparison.OrdinalIgnoreCase)));
        if (sysCategory  is null && codingCategoryFound)
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