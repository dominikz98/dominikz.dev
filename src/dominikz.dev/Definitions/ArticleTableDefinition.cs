using dominikz.dev.Components.Tables;
using dominikz.dev.Utils;
using dominikz.shared;
using dominikz.shared.Enums;
using dominikz.shared.ViewModels.Blog;

namespace dominikz.dev.Definitions;

public static class ArticleTableDefinition
{
    public static List<ColumnDefinition<ArticleVm>> Columns
        => new()
        {
            new(nameof(ArticleVm.Title), x => x.Title),
            new(nameof(ArticleVm.Author), x => x.Author!.Name) { Actions = ColumnActionFlags.HIDE_ON_MOBILE },
            new(nameof(ArticleVm.Category), x => x.Category) { Formatter = (x) => EnumFormatter.ToString(((ArticleCategoryEnum)(x ?? string.Empty)).GetFlags().ToArray()[1..]) },
            new(nameof(ArticleVm.PublishDate), x => x.PublishDate) { Formatter = (x) => $"{x:yyyy.MM.dd}", Actions = ColumnActionFlags.HIDE_ON_MOBILE }
        };
}