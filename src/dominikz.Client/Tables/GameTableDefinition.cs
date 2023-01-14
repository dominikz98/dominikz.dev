using dominikz.Client.Components.Tables;
using dominikz.Client.Utils;
using dominikz.Domain.Enums;
using dominikz.Domain.Extensions;
using dominikz.Domain.ViewModels.Media;

namespace dominikz.Client.Tables;

public static class GameTableDefinition
{
    public static List<ColumnDefinition<GameVm>> Columns
    {
        get => new()
        {
            new (nameof(GameVm.Title), x => x.Title),
            new (nameof(GameVm.Year), x => x.Year) {Actions = ColumnActionFlags.HideOnMobile},
            new (nameof(MovieVm.Genres), x => x.Genres) {  Formatter = (x) => EnumFormatter.ToString(((GameGenresFlags)(x ?? string.Empty)).GetFlags().ToArray()[1..]) },
            new (nameof(GameVm.Platform), x => x.Platform) { Formatter = (platform) => EnumFormatter.ToString((GamePlatformEnum)platform!), Actions = ColumnActionFlags.HideOnMobile },
            new (nameof(GameVm.PublishDate), x => x.PublishDate) {  Formatter = (x) => $"{x:yyyy.MM.dd}", Actions = ColumnActionFlags.HideOnMobile }
        };
    }
}
