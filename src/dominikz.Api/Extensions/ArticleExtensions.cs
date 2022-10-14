using dominikz.api.Models;
using dominikz.kernel.Endpoints;
using dominikz.kernel.ViewModels;

namespace dominikz.api.Extensions;

public static class ArticleExtensions
{
    public static IQueryable<Article> Search(this IQueryable<Article> query, ArticleFilter filter)
    {
        if (filter.Id != null)
            return query.Where(x => x.Id == filter.Id);

        if (filter.Category != ArticleCategoryEnum.ALL)
            query = query.Where(x => x.Category == filter.Category);

        if (!string.IsNullOrWhiteSpace(filter.Text))
            query = query.Where(x => x.Title.Contains(filter.Text)
                    || x.Tags.Any(y => y.Contains(filter.Text))
                    || x.Author!.Name.Contains(filter.Text));

        return query.OrderByDescending(x => x.Timestamp)
            .ThenBy(x => x.Title);
    }
}
