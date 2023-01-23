using dominikz.Client.Components.Tables;
using dominikz.Client.Utils;
using dominikz.Domain.Enums;
using dominikz.Domain.Enums.Blog;
using dominikz.Domain.Extensions;
using dominikz.Domain.ViewModels.Blog;
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