
using dominikz.dev.Components;
using dominikz.dev.Components.Tables;
using dominikz.dev.Utils;
using dominikz.shared;
using dominikz.shared.Contracts;
using dominikz.shared.ViewModels;

namespace dominikz.dev.Definitions;

public static class BookTableDefinition
{
    public static List<string> OrderKeys
    {
        get => new()
        {
            nameof(BookVM.Title),
            nameof(BookVM.Timestamp),
            nameof(BookVM.Year)
        };
    }

    public static List<ColumnDefinition<BookVM>> Columns
    {
        get => new()
        {
            new (nameof(BookVM.Title), x => x.Title),
            new (nameof(BookVM.Year), x => x.Year) {Actions = ColumnActionFlags.HIDE_ON_MOBILE},
            new (nameof(BookVM.Author), x => x.Author),
            new (nameof(MovieVM.Genres), x => x.Genres) {  Formatter = (x) => EnumConverter.ToString(((BookGenresFlags)(x ?? string.Empty)).GetFlags().ToArray()[1..]) },
            new (nameof(BookVM.Language), x => x.Language) { Formatter = (language) => EnumConverter.ToString((BookLanguageEnum)language!), Actions = ColumnActionFlags.HIDE_ON_MOBILE },
            new (nameof(BookVM.Timestamp), x => x.Timestamp) {  Formatter = (x) => $"{x:yyyy.MM.dd}", Actions = ColumnActionFlags.HIDE_ON_MOBILE }
        };
    }

    public static IOrderedEnumerable<BookVM> OrderByKey(this List<BookVM> query, OrderInfo order)
    {
        if (order.Key.Equals(nameof(BookVM.Timestamp), StringComparison.OrdinalIgnoreCase))
        {
            if (order.Direction == OrderDirection.Ascending)
                return query.OrderBy(x => x.Timestamp);
            else
                return query.OrderByDescending(x => x.Timestamp);
        }
        else if (order.Key.Equals(nameof(BookVM.Year), StringComparison.OrdinalIgnoreCase))
        {
            if (order.Direction == OrderDirection.Ascending)
                return query.OrderBy(x => x.Year);
            else
                return query.OrderByDescending(x => x.Year);
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
