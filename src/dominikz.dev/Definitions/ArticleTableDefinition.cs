using dominikz.dev.Components;
using dominikz.dev.Components.Tables;
using dominikz.dev.Utils;
using dominikz.shared;
using dominikz.shared.Contracts;
using dominikz.shared.ViewModels;

namespace dominikz.dev.Definitions;

public static class ArticleTableDefinition
{
    public static List<string> OrderKeys
    {
        get => new()
        {
            nameof(ArticleListVm.Title),
            nameof(ArticleListVm.Timestamp)
        };
    }

    public static List<ColumnDefinition<ArticleListVm>> Columns
    {
        get => new()
        {
            new (nameof(ArticleListVm.Title), x => x.Title),
            new (nameof(ArticleListVm.Author), x => x.Author!.Name) { Actions = ColumnActionFlags.HIDE_ON_MOBILE },
            new (nameof(ArticleListVm.Category), x => x.Category) {  Formatter = (x) => EnumConverter.ToString(((ArticleCategoryEnum)(x ?? string.Empty)).GetFlags().ToArray()[1..]) },
            new (nameof(ArticleListVm.Timestamp), x => x.Timestamp) {  Formatter = (x) => $"{x:yyyy.MM.dd}", Actions = ColumnActionFlags.HIDE_ON_MOBILE }
        };
    }

    public static IOrderedEnumerable<ArticleListVm> OrderByKey(this List<ArticleListVm> query, OrderInfo order)
    {
        if (order.Key.Equals(nameof(ArticleListVm.Timestamp), StringComparison.OrdinalIgnoreCase))
        {
            if (order.Direction == OrderDirection.Ascending)
                return query.OrderBy(x => x.Timestamp);
            else
                return query.OrderByDescending(x => x.Timestamp);
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
