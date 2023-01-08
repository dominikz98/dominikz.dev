using dominikz.dev.Components.Tables;
using dominikz.dev.Utils;
using dominikz.shared;
using dominikz.shared.Enums;
using dominikz.shared.ViewModels.Media;

namespace dominikz.dev.Definitions;

public static class MovieTableDefinition
{
    public static List<ColumnDefinition<MovieVm>> Columns
    {
        get => new()
        {
            new (nameof(MovieVm.Title), x => x.Title),
            new (nameof(MovieVm.Year), x => x.Year) { Actions = ColumnActionFlags.HIDE_ON_MOBILE },
            new (nameof(MovieVm.Genres), x => x.Genres) {  Formatter = (x) => EnumFormatter.ToString(((MovieGenresFlags)(x ?? string.Empty)).GetFlags().ToArray()[1..]) },
            new (nameof(MovieVm.PublishDate), x => x.PublishDate) {  Formatter = (x) => $"{x:yyyy.MM.dd}", Actions = ColumnActionFlags.HIDE_ON_MOBILE },
            new (nameof(MovieVm.Rating), x => x.Rating) {  Formatter = (x) => $"{x}/100" },
        };
    }
}
