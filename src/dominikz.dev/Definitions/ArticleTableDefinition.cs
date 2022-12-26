using dominikz.dev.Components.Tables;
using dominikz.dev.Utils;
using dominikz.shared;
using dominikz.shared.Contracts;
using dominikz.shared.ViewModels;

namespace dominikz.dev.Definitions;

public static class ArticleTableDefinition
{
    public static List<ColumnDefinition<ArticleListVm>> Columns
        => new()
        {
            new(nameof(ArticleListVm.Title), x => x.Title),
            new(nameof(ArticleListVm.Author), x => x.Author!.Name) { Actions = ColumnActionFlags.HIDE_ON_MOBILE },
            new(nameof(ArticleListVm.Category), x => x.Category) { Formatter = (x) => EnumFormatter.ToString(((ArticleCategoryEnum)(x ?? string.Empty)).GetFlags().ToArray()[1..]) },
            new(nameof(ArticleListVm.Timestamp), x => x.Timestamp) { Formatter = (x) => $"{x:yyyy.MM.dd}", Actions = ColumnActionFlags.HIDE_ON_MOBILE }
        };
}