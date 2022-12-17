using dominikz.dev.Components;
using dominikz.dev.Components.Tables;
using dominikz.dev.Utils;
using dominikz.shared;
using dominikz.shared.Contracts;
using dominikz.shared.ViewModels;

namespace dominikz.dev.Definitions;

public static class GameTableDefinition
{
    public static List<string> OrderKeys
    {
        get => new()
        {
            nameof(GameVM.Title),
            nameof(GameVM.Timestamp),
            nameof(BookVM.Year)
        };
    }

    public static List<ColumnDefinition<GameVM>> Columns
    {
        get => new()
        {
            new (nameof(GameVM.Title), x => x.Title),
            new (nameof(GameVM.Year), x => x.Year) {Actions = ColumnActionFlags.HIDE_ON_MOBILE},
            new (nameof(MovieVM.Genres), x => x.Genres) {  Formatter = (x) => EnumConverter.ToString(((GameGenresFlags)(x ?? string.Empty)).GetFlags().ToArray()[1..]) },
            new (nameof(GameVM.Platform), x => x.Platform) { Formatter = (platform) => EnumConverter.ToString((GamePlatformEnum)platform!), Actions = ColumnActionFlags.HIDE_ON_MOBILE },
            new (nameof(GameVM.Timestamp), x => x.Timestamp) {  Formatter = (x) => $"{x:yyyy.MM.dd}", Actions = ColumnActionFlags.HIDE_ON_MOBILE }
        };
    }

    public static IOrderedEnumerable<GameVM> OrderByKey(this List<GameVM> query, OrderInfo order)
    {
        if (order.Key.Equals(nameof(GameVM.Timestamp), StringComparison.OrdinalIgnoreCase))
        {
            if (order.Direction == OrderDirection.Ascending)
                return query.OrderBy(x => x.Timestamp);
            else
                return query.OrderByDescending(x => x.Timestamp);
        }
        else if (order.Key.Equals(nameof(GameVM.Year), StringComparison.OrdinalIgnoreCase))
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
