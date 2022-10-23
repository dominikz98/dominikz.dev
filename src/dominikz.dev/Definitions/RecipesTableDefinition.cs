using dominikz.dev.Components;
using dominikz.dev.Components.Tables;
using dominikz.kernel.ViewModels;

namespace dominikz.dev.Definitions;

public static class RecipesTableDefinition
{
    public static List<string> OrderKeys
    {
        get => new()
        {
            nameof(RecipeVM.Title), 
            nameof(RecipeVM.PricePerPortion), 
            nameof(RecipeVM.Portions), 
            nameof(RecipeVM.Duration), 
            nameof(RecipeVM.FoodCount)
        };
    }

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

    public static IOrderedEnumerable<RecipeVM> OrderByKey(this List<RecipeVM> query, OrderInfo order)
    {
        if (order.Key.Equals(nameof(RecipeVM.PricePerPortion), StringComparison.OrdinalIgnoreCase))
        {
            if (order.Direction == OrderDirection.Ascending)
                return query.OrderBy(x => x.PricePerPortion);
            else
                return query.OrderByDescending(x => x.PricePerPortion);
        }
        else if (order.Key.Equals(nameof(RecipeVM.Portions), StringComparison.OrdinalIgnoreCase))
        {
            if (order.Direction == OrderDirection.Ascending)
                return query.OrderBy(x => x.Portions);
            else
                return query.OrderByDescending(x => x.Portions);
        }
        else if (order.Key.Equals(nameof(RecipeVM.Duration), StringComparison.OrdinalIgnoreCase))
        {
            if (order.Direction == OrderDirection.Ascending)
                return query.OrderBy(x => x.Duration);
            else
                return query.OrderByDescending(x => x.Duration);
        }
        else if (order.Key.Equals(nameof(RecipeVM.FoodCount), StringComparison.OrdinalIgnoreCase))
        {
            if (order.Direction == OrderDirection.Ascending)
                return query.OrderBy(x => x.FoodCount);
            else
                return query.OrderByDescending(x => x.FoodCount);
        }
        else
        {
            if (order.Direction == OrderDirection.Ascending)
                return query.OrderBy(x => x.Title);
            else
                return query.OrderByDescending(x => x.Title);
        }
    }
}
