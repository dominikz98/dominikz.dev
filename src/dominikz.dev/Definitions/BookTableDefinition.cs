using dominikz.dev.Components.Tables;
using dominikz.dev.Utils;
using dominikz.shared;
using dominikz.shared.Contracts;
using dominikz.shared.ViewModels;

namespace dominikz.dev.Definitions;

public static class BookTableDefinition
{
    public static List<ColumnDefinition<BookVM>> Columns
    {
        get => new()
        {
            new (nameof(BookVM.Title), x => x.Title),
            new (nameof(BookVM.Year), x => x.Year) {Actions = ColumnActionFlags.HIDE_ON_MOBILE},
            new (nameof(BookVM.Author), x => x.Author),
            new (nameof(MovieVM.Genres), x => x.Genres) {  Formatter = (x) => EnumFormatter.ToString(((BookGenresFlags)(x ?? string.Empty)).GetFlags().ToArray()[1..]) },
            new (nameof(BookVM.Language), x => x.Language) { Formatter = (language) => EnumFormatter.ToString((BookLanguageEnum)language!), Actions = ColumnActionFlags.HIDE_ON_MOBILE },
            new (nameof(BookVM.Timestamp), x => x.Timestamp) {  Formatter = (x) => $"{x:yyyy.MM.dd}", Actions = ColumnActionFlags.HIDE_ON_MOBILE }
        };
    }
}
