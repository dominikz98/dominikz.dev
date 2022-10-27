using dominikz.dev.Components;
using dominikz.dev.Components.Tables;
using dominikz.dev.Utils;
using dominikz.kernel;
using dominikz.kernel.Contracts;
using dominikz.kernel.ViewModels;

namespace dominikz.dev.Definitions;

public static class ArticleTableDefinition
{
    public static List<string> OrderKeys
    {
        get => new()
        {
            nameof(ArticleListVM.Title),
            nameof(ArticleListVM.Timestamp)
        };
    }

    public static List<ColumnDefinition<ArticleListVM>> Columns
    {
        get => new()
        {
            new (nameof(ArticleListVM.Title), x => x.Title),
            new (nameof(ArticleListVM.Author), x => x.Author!.Name) { Actions = ColumnActionFlags.HIDE_ON_MOBILE },
            new (nameof(ArticleListVM.Category), x => x.Category) {  Formatter = (x) => EnumConverter.ToString(((ArticleCategoryEnum)(x ?? string.Empty)).GetFlags().ToArray()[1..]) },
            new (nameof(ArticleListVM.Timestamp), x => x.Timestamp) {  Formatter = (x) => $"{x:yyyy.MM.dd}", Actions = ColumnActionFlags.HIDE_ON_MOBILE }
        };
    }

    public static IOrderedEnumerable<ArticleListVM> OrderByKey(this List<ArticleListVM> query, OrderInfo order)
    {
        if (order.Key.Equals(nameof(ArticleListVM.Timestamp), StringComparison.OrdinalIgnoreCase))
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
