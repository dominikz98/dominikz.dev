using dominikz.dev.Components.Tables;
using dominikz.dev.Utils;
using dominikz.shared;
using dominikz.shared.Enums;
using dominikz.shared.ViewModels.Media;

namespace dominikz.dev.Definitions;

public static class GameTableDefinition
{
    public static List<ColumnDefinition<GameVm>> Columns
    {
        get => new()
        {
            new (nameof(GameVm.Title), x => x.Title),
            new (nameof(GameVm.Year), x => x.Year) {Actions = ColumnActionFlags.HIDE_ON_MOBILE},
            new (nameof(MovieVm.Genres), x => x.Genres) {  Formatter = (x) => EnumFormatter.ToString(((GameGenresFlags)(x ?? string.Empty)).GetFlags().ToArray()[1..]) },
            new (nameof(GameVm.Platform), x => x.Platform) { Formatter = (platform) => EnumFormatter.ToString((GamePlatformEnum)platform!), Actions = ColumnActionFlags.HIDE_ON_MOBILE },
            new (nameof(GameVm.PublishDate), x => x.PublishDate) {  Formatter = (x) => $"{x:yyyy.MM.dd}", Actions = ColumnActionFlags.HIDE_ON_MOBILE }
        };
    }
}
