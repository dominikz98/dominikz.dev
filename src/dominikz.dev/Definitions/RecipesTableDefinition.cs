using dominikz.dev.Components.Tables;
using dominikz.shared.ViewModels;

namespace dominikz.dev.Definitions;

public static class RecipesTableDefinition
{
    public static List<ColumnDefinition<RecipeVM>> Columns
    {
        get => new()
        {
            new (nameof(RecipeVM.Title), x => x.Title),
            new (nameof(RecipeVM.Portions), x => x.Portions),
            new ("Price / Portion", x => x.PricePerPortion) { Formatter = x => $"{x:c2}", Actions = ColumnActionFlags.HIDE_ON_MOBILE },
            new ("Components", x => x.FoodCount) { Actions = ColumnActionFlags.HIDE_ON_MOBILE },
            new (nameof(RecipeVM.Duration), x => x.Duration)
        };
    }
}
