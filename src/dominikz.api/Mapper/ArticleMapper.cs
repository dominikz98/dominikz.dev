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

    public static IQueryable<ArticleListVm> MapToVm(this IQueryable<Article> query)
        => query.Select(article => new ArticleListVm()
        {
            Id = article.Id,
            Author = article.Author!.MapToVm(),
            Image = article.File!.MapToVm(),
            Title = article.Title,
            Timestamp = article.Timestamp,
            Category = article.Category,
            Source = ArticleSource.DZ,
            Path = $"~/blog/{article.Id}"
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
            Source = ArticleSource.Noobit
        });

    public static IEnumerable<ArticleListVm> MapToVm(this IEnumerable<SyndicationItem> query)
        => query.Select(article => new ArticleListVm()
        {
            Id = Guid.Empty,
            Title = article.Title.Text,
            Timestamp = article.PublishDate.Date,
            Category = ArticleCategoryEnum.ALL,
            Path = article.Links.FirstOrDefault()?.Uri.ToString() ?? string.Empty,
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
                Id = MarkusLieblDefaultImageId,
                Category = FileCategoryEnum.IMAGE,
                Extension = FileExtensionEnum.JPG
            },
            Source = ArticleSource.Medlan
        });

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
}