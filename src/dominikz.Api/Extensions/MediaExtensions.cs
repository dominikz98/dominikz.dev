using dominikz.Api.Models;
using dominikz.kernel.Endpoints;
using dominikz.kernel.ViewModels;

namespace dominikz.Api.Extensions;

public static class MediaExtensions
{
    public static IQueryable<Media> Search(this IQueryable<Media> query, MediaFilter filter)
    {
        if (filter.Id != null)
            return query.Where(x => x.Id == filter.Id);

        if (filter.Category != MediaCategoryEnum.ALL)
            query = query.Where(x => x.Category == filter.Category);

        if (filter.Genre != MediaGenre.ALL)
            query = query.Where(x => x.Genres.HasFlag(filter.Genre));

        if (!string.IsNullOrWhiteSpace(filter.Text))
            query = query.Where(x => x.Title.Contains(filter.Text, StringComparison.OrdinalIgnoreCase)
                        || x.Author!.Name.Contains(filter.Text, StringComparison.OrdinalIgnoreCase));

        // add offset limit
        query = query.OrderByDescending(x => x.Timestamp)
            .ThenBy(x => x.Title)
            .ThenBy(x => x.Rating)
            .Skip(filter.Index ?? 0);

        if (filter.Count is not null)
            query = query.Take(filter.Count.Value);

        return query;
    }
}
