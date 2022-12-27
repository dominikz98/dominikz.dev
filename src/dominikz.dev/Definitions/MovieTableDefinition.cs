using dominikz.dev.Components.Tables;
using dominikz.dev.Utils;
using dominikz.shared;
using dominikz.shared.Contracts;
using dominikz.shared.ViewModels.Media;

namespace dominikz.dev.Definitions;

public static class MovieTableDefinition
{
    public static List<ColumnDefinition<MovieVM>> Columns
    {
        get => new()
        {
            new (nameof(MovieVM.Title), x => x.Title),
            new (nameof(MovieVM.Year), x => x.Year) { Actions = ColumnActionFlags.HIDE_ON_MOBILE },
            new (nameof(MovieVM.Genres), x => x.Genres) {  Formatter = (x) => EnumFormatter.ToString(((MovieGenresFlags)(x ?? string.Empty)).GetFlags().ToArray()[1..]) },
            new (nameof(MovieVM.Timestamp), x => x.Timestamp) {  Formatter = (x) => $"{x:yyyy.MM.dd}", Actions = ColumnActionFlags.HIDE_ON_MOBILE },
            new (nameof(MovieVM.Rating), x => x.Rating) {  Formatter = (x) => $"{x}/100" },
        };
    }
}
