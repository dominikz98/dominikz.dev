using dominikz.Domain.Enums.Blog;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Blog;
using dominikz.Infrastructure.Clients.Noobit;

namespace dominikz.Infrastructure.Mapper;

public static class ArticleMapper
{
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

    public static IQueryable<ArticleVm> MapToVm(this IQueryable<ExtArticleShadow> query)
        => query.Select(shadow => new ArticleVm()
        {
            Id = Guid.NewGuid(),
            Title = shadow.Title,
            PublishDate = shadow.Date.ToDateTime(TimeOnly.MinValue),
            Category = shadow.Category,
            Path = shadow.Url,
            ImageUrl = shadow.ImageUrl,
            Source = shadow.Source
        });
    
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
}