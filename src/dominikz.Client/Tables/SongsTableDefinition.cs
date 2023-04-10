using dominikz.Client.Components.Tables;
using dominikz.Domain.ViewModels.Songs;

namespace dominikz.Client.Tables;

public static class SongsTableDefinition
{
    public static List<ColumnDefinition<SongVm>> Columns
        => new()
        {
            new(nameof(SongVm.Name), x => x.Name)
        };
}