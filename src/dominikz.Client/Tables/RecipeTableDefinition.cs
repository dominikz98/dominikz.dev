using dominikz.Client.Components.Tables;
using dominikz.Client.Utils;
using dominikz.Domain.Enums.Cookbook;
using dominikz.Domain.ViewModels.Cookbook;

namespace dominikz.Client.Tables;

public static class RecipeTableDefinition
{
    public static List<ColumnDefinition<RecipeListVm>> Columns
        => new()
        {
            new(nameof(RecipeListVm.Name), x => x.Name),
            new(nameof(RecipeListVm.Type), x => x.Type) { Formatter = x => EnumFormatter.ToString((RecipeType)(x ?? string.Empty)), Actions = ColumnActionFlags.HideOnMobile },
            new(nameof(RecipeListVm.Duration), x => x.Duration) { Formatter = x => $"{x}m" },
            new(nameof(RecipeListVm.Flags), x => x.Flags) { Formatter = x => x == null ? string.Empty : string.Join(" ", ((List<RecipeFlags>)x).Select(EnumFormatter.ToIcon)) },
        };
}