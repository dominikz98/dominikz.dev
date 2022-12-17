using dominikz.dev.Components;
using dominikz.dev.Components.Tables;
using dominikz.dev.Utils;
using dominikz.shared;
using dominikz.shared.Contracts;
using dominikz.shared.ViewModels;

namespace dominikz.dev.Definitions;

public static class MovieTableDefinition
{
    public static List<string> OrderKeys
    {
        get => new()
        {
            nameof(MovieVM.Title),
            nameof(MovieVM.Rating),
            nameof(MovieVM.Timestamp)
        };
    }

    public static List<ColumnDefinition<MovieVM>> Columns
    {
        get => new()
        {
            new (nameof(MovieVM.Title), x => x.Title),
            new (nameof(MovieVM.Year), x => x.Year) { Actions = ColumnActionFlags.HIDE_ON_MOBILE },
            new (nameof(MovieVM.Genres), x => x.Genres) {  Formatter = (x) => EnumConverter.ToString(((MovieGenresFlags)(x ?? string.Empty)).GetFlags().ToArray()[1..]) },
            new (nameof(MovieVM.Timestamp), x => x.Timestamp) {  Formatter = (x) => $"{x:yyyy.MM.dd}", Actions = ColumnActionFlags.HIDE_ON_MOBILE },
            new (nameof(MovieVM.Rating), x => x.Rating) {  Formatter = (x) => $"{x}/100" },
        };
    }

    public static IOrderedEnumerable<MovieVM> OrderByKey(this List<MovieVM> query, OrderInfo order)
    {
        if (order.Key.Equals(nameof(MovieVM.Timestamp), StringComparison.OrdinalIgnoreCase))
        {
            if (order.Direction == OrderDirection.Ascending)
                return query.OrderBy(x => x.Timestamp);
            else
                return query.OrderByDescending(x => x.Timestamp);
        }
        else if (order.Key.Equals(nameof(MovieVM.Rating), StringComparison.OrdinalIgnoreCase))
        {
            if (order.Direction == OrderDirection.Ascending)
                return query.OrderBy(x => x.Rating);
            else
                return query.OrderByDescending(x => x.Rating);
        }
        else
        {
            if (order.Direction == OrderDirection.Ascending)
                return query.OrderBy(x => x.Title);
            else
                return query.OrderByDescending(x => x.Title);
        }
    }
}
