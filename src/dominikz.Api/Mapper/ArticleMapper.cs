using dominikz.api.Models;
using dominikz.api.Provider;
using dominikz.kernel.Contracts;
using dominikz.kernel.ViewModels;

namespace dominikz.api.Mapper;

public static class ArticleMapper
{
    public static readonly Guid TobiasHaimerlId = Guid.Parse("c79efe4c-6b9e-4ce9-9e09-b85777082b32");
    public static readonly Guid DominikZettlId = Guid.Parse("4ed3e8d2-44e0-11ed-9076-00d861ff2f96");

    public static IQueryable<ArticleListVM> MapToVM(this IQueryable<Article> query)
        => query.Select(article => new ArticleListVM()
        {
            Id = article.Id,
            Author = article.Author!.MapToVM(),
            Image = article.File!.MapToVM(),
            Title = article.Title,
            Timestamp = article.Timestamp,
            Category = article.Category
        });

    public static IEnumerable<ArticleListVM> MapToVM(this IEnumerable<NoobitArticleVM> query)
       => query.Select(article => new ArticleListVM()
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
                   Category = FileCagetoryEnum.IMAGE,
                   Extension = FileExtensionEnum.WEBP
               }
           },
           Image = new FileVM()
           {
               Id = article.Id,
               Category = FileCagetoryEnum.IMAGE,
               Extension = FileExtensionEnum.WEBP,
               Url = article.ImageUrl
           }
       });

    public static IQueryable<ArticleDetailVM> MapToDetailVM(this IQueryable<Article> query)
        => query.Select(article => new ArticleDetailVM()
        {
            Id = article.Id,
            Author = article.Author!.MapToVM(),
            Image = article.File!.MapToVM(),
            Title = article.Title,
            Timestamp = article.Timestamp,
            Category = article.Category,
            Tags = article.Tags,
            Text = article.MDText
        });
}
