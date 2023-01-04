using System.ServiceModel.Syndication;
using dominikz.api.Models;
using dominikz.api.Provider.Noobit;
using dominikz.shared;
using dominikz.shared.Contracts;
using dominikz.shared.ViewModels;
using dominikz.shared.ViewModels.Blog;

namespace dominikz.api.Mapper;

public static class ArticleMapper
{
    private static readonly Guid TobiasHaimerlId = Guid.Parse("c79efe4c-6b9e-4ce9-9e09-b85777082b32");
    private static readonly Guid MarkusLieblId = Guid.Parse("3fd2fb6d-7cb7-11ed-8bce-cffa786aaa15");
    private static readonly Guid DominikZettlId = Guid.Parse("4ed3e8d2-44e0-11ed-9076-00d861ff2f96");

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

    public static void ApplyChanges(this Article original, Article current, string contentType)
    {
        original.Id = current.Id;
        original.Title = current.Title;
        original.PublishDate = current.PublishDate;
        original.AuthorId = current.AuthorId;
        original.Category = current.Category;
        original.Tags = current.Tags;
        original.HtmlText = current.HtmlText;
        original.FileId = current.Id;
        original.File ??= new StorageFile();
        original.File.Id = current.Id;
        
        var extension = FileIdentifier.GetExtensionByContentType(contentType);
        original.File.Category = FileIdentifier.GetCategoryByExtension(extension);
        original.File.Extension = extension;
    }

    public static Article MapToModel(this EditArticleVm vm, string contentType)
    {
        var extension = FileIdentifier.GetExtensionByContentType(contentType);
        return new()
        {
            Id = vm.Id,
            Title = vm.Title,
            PublishDate = vm.PublishDate,
            AuthorId = DominikZettlId,
            Category = vm.Category,
            Tags = vm.Tags.ToList(),
            HtmlText = vm.HtmlText,
            FileId = vm.Id,
            File = new StorageFile()
            {
                Id = vm.Id,
                Category = FileIdentifier.GetCategoryByExtension(extension),
                Extension = extension
            }
        };
    }

    public static Article MapToModel(this AddArticleVm vm, string contentType)
    {
        var extension = FileIdentifier.GetExtensionByContentType(contentType);
        var id = Guid.NewGuid();
        return new()
        {
            Id = id,
            Title = vm.Title,
            PublishDate = vm.PublishDate,
            AuthorId = DominikZettlId,
            Category = vm.Category,
            Tags = vm.Tags.ToList(),
            HtmlText = vm.HtmlText,
            FileId = id,
            File = new StorageFile()
            {
                Id = id,
                Category = FileIdentifier.GetCategoryByExtension(extension),
                Extension = extension
            }
        };
    }

    public static IQueryable<ArticleVm> MapToVm(this IQueryable<Article> query)
        => query.Select(article => new ArticleVm()
        {
            Id = article.Id,
            Author = article.Author!.MapToVm(),
            ImageUrl = article.File!.Id.ToString(),
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
            Author = new PersonVM()
            {
                Id = TobiasHaimerlId,
                Name = "Tobias Haimerl",
                ImageUrl = TobiasHaimerlId.ToString()
            },
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
                Author = new PersonVM()
                {
                    Id = MarkusLieblId,
                    Name = "Markus Liebl",
                    ImageUrl = MarkusLieblId.ToString()
                },
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
            HtmlText = article.HtmlText,
            ImageId = article.File!.Id
        });

    public static IQueryable<ArticleViewVm> MapToViewVm(this IQueryable<Article> query)
        => query.Select(article => article.MapToViewVm());

    private static ArticleViewVm MapToViewVm(this Article article)
        => new()
        {
            Id = article.Id,
            Author = article.Author!.MapToVm(),
            ImageUrl = article.File!.Id.ToString(),
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