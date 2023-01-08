using dominikz.dev.Components.Tables;
using dominikz.dev.Utils;
using dominikz.shared;
using dominikz.shared.Enums;
using dominikz.shared.ViewModels.Media;

namespace dominikz.dev.Definitions;

public static class BookTableDefinition
{
    public static List<ColumnDefinition<BookVm>> Columns
    {
        get => new()
        {
            new (nameof(BookVm.Title), x => x.Title),
            new (nameof(BookVm.Year), x => x.Year) {Actions = ColumnActionFlags.HIDE_ON_MOBILE},
            new (nameof(BookVm.Author), x => x.Author),
            new (nameof(MovieVm.Genres), x => x.Genres) {  Formatter = (x) => EnumFormatter.ToString(((BookGenresFlags)(x ?? string.Empty)).GetFlags().ToArray()[1..]) },
            new (nameof(BookVm.Language), x => x.Language) { Formatter = (language) => EnumFormatter.ToString((BookLanguageEnum)language!), Actions = ColumnActionFlags.HIDE_ON_MOBILE },
            new (nameof(BookVm.PublishDate), x => x.PublishDate) {  Formatter = (x) => $"{x:yyyy.MM.dd}", Actions = ColumnActionFlags.HIDE_ON_MOBILE }
        };
    }
}
