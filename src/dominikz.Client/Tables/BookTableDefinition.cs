using dominikz.Client.Components.Tables;
using dominikz.Client.Utils;
using dominikz.Domain.Enums.Media;
using dominikz.Domain.Extensions;
using dominikz.Domain.ViewModels.Media;

namespace dominikz.Client.Tables;

public static class BookTableDefinition
{
    public static List<ColumnDefinition<BookVm>> Columns
    {
        get => new()
        {
            new (nameof(BookVm.Title), x => x.Title),
            new (nameof(BookVm.Year), x => x.Year) {Actions = ColumnActionFlags.HideOnMobile},
            new (nameof(BookVm.Author), x => x.Author),
            new (nameof(MovieVm.Genres), x => x.Genres) {  Formatter = x => EnumFormatter.ToString(((BookGenresFlags)(x ?? string.Empty)).GetFlags().ToArray()[1..]) },
            new (nameof(BookVm.Language), x => x.Language) { Formatter = language => EnumFormatter.ToString((BookLanguageEnum)language!), Actions = ColumnActionFlags.HideOnMobile },
            new (nameof(BookVm.PublishDate), x => x.PublishDate) {  Formatter = x => $"{x:yyyy.MM.dd}", Actions = ColumnActionFlags.HideOnMobile }
        };
    }
}
