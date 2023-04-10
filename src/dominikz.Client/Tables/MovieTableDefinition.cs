using dominikz.Client.Components.Tables;
using dominikz.Client.Utils;
using dominikz.Domain.Enums.Media;
using dominikz.Domain.Extensions;
using dominikz.Domain.ViewModels.Media;

namespace dominikz.Client.Tables;

public static class MovieTableDefinition
{
    public static List<ColumnDefinition<MovieVm>> Columns
    {
        get => new()
        {
            new (nameof(MovieVm.Title), x => x.Title),
            new (nameof(MovieVm.Year), x => x.Year) { Actions = ColumnActionFlags.HideOnMobile },
            new (nameof(MovieVm.Genres), x => x.Genres) {  Formatter = x => EnumFormatter.ToString(((MovieGenresFlags)(x ?? string.Empty)).GetFlags().ToArray()[1..]) },
            new (nameof(MovieVm.PublishDate), x => x.PublishDate) {  Formatter = x => $"{x:yyyy.MM.dd}", Actions = ColumnActionFlags.HideOnMobile },
            new (nameof(MovieVm.Rating), x => x.Rating) {  Formatter = x => $"{x}/100" },
        };
    }
}
