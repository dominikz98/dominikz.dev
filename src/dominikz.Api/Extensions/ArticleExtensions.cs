using dominikz.Api.Models;
using dominikz.kernel.Endpoints;
using dominikz.kernel.ViewModels;

namespace dominikz.Api.Extensions;

public static class ArticleExtensions
{
    public static IQueryable<Article> Search(this IQueryable<Article> query, ArticleFilter filter)
    {
        if (filter.Id != null)
            return query.Where(x => x.Id == filter.Id);

        if (filter.Category != ArticleCategoryEnum.ALL)
            query = query.Where(x => x.Category == filter.Category);

        if (!string.IsNullOrWhiteSpace(filter.Text))
            query = query.Where(x => x.Title.Contains(filter.Text, StringComparison.OrdinalIgnoreCase)
                    || x.Tags.Any(y => y.Contains(filter.Text, StringComparison.OrdinalIgnoreCase))
                    || x.Author!.Name.Contains(filter.Text, StringComparison.OrdinalIgnoreCase));

        return query.OrderByDescending(x => x.Timestamp)
            .ThenBy(x => x.Title);
    }
}
