using dominikz.api.Models;
using dominikz.kernel.ViewModels;

namespace dominikz.api.Mapper;

public static class ArticleMapper
{
    public static IQueryable<ArticleListVM> MapToVM(this IQueryable<Article> query)
        => query.Select(article => new ArticleListVM()
        {
            Id = article.Id,
            Author = article.Author!.MapToVM(),
            Image = article.File!.MapToVM(),
            Title = article.Title,
            Timestamp = article.Timestamp,
            Category = article.Category,
            Available = article.MDText.Length > 1
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
