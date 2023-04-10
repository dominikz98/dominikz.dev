using dominikz.Client.Components.Tables;
using dominikz.Client.Utils;
using dominikz.Domain.Enums.Blog;
using dominikz.Domain.Extensions;
using dominikz.Domain.ViewModels.Blog;

namespace dominikz.Client.Tables;

public static class ArticleTableDefinition
{
    public static List<ColumnDefinition<ArticleVm>> Columns
        => new()
        {
            new(nameof(ArticleVm.Title), x => x.Title),
            new("Author", x => AuthorHelper.GetNameBySource(x.Source)) { Actions = ColumnActionFlags.HideOnMobile },
            new(nameof(ArticleVm.Category), x => x.Category) { Formatter = x => EnumFormatter.ToString(((ArticleCategoryEnum)(x ?? string.Empty)).GetFlags().ToArray()[1..]) },
            new(nameof(ArticleVm.PublishDate), x => x.PublishDate) { Formatter = x => $"{x:yyyy.MM.dd}", Actions = ColumnActionFlags.HideOnMobile }
        };
}