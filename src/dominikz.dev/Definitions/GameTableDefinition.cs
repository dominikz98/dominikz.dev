using dominikz.dev.Components.Tables;
using dominikz.dev.Utils;
using dominikz.shared;
using dominikz.shared.Contracts;
using dominikz.shared.ViewModels;

namespace dominikz.dev.Definitions;

public static class GameTableDefinition
{
    public static List<ColumnDefinition<GameVM>> Columns
    {
        get => new()
        {
            new (nameof(GameVM.Title), x => x.Title),
            new (nameof(GameVM.Year), x => x.Year) {Actions = ColumnActionFlags.HIDE_ON_MOBILE},
            new (nameof(MovieVM.Genres), x => x.Genres) {  Formatter = (x) => EnumFormatter.ToString(((GameGenresFlags)(x ?? string.Empty)).GetFlags().ToArray()[1..]) },
            new (nameof(GameVM.Platform), x => x.Platform) { Formatter = (platform) => EnumFormatter.ToString((GamePlatformEnum)platform!), Actions = ColumnActionFlags.HIDE_ON_MOBILE },
            new (nameof(GameVM.Timestamp), x => x.Timestamp) {  Formatter = (x) => $"{x:yyyy.MM.dd}", Actions = ColumnActionFlags.HIDE_ON_MOBILE }
        };
    }
}
