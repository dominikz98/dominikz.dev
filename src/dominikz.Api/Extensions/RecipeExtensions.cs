using dominikz.api.Models;
using dominikz.kernel.Endpoints;
using dominikz.kernel.ViewModels;

namespace dominikz.api.Extensions;

public static class RecipeExtensions
{
    public static IQueryable<Recipe> Search(this IQueryable<Recipe> query, RecipesFilter filter)
    {
        if (filter.Category != RecipeCategoryFlags.ALL)
            query = query.Where(x => x.Categories.HasFlag(filter.Category));

        if (!string.IsNullOrWhiteSpace(filter.Text))
            query = query.Where(x => x.Title.Contains(filter.Text));

        if (filter.FoodIds.Count > 0)
            query = query.Where(x => x.RecipesFoodsMappings.Any(y => filter.FoodIds.Contains(y.FoodId)));

        return query.OrderByDescending(x => x.Title);
    }
}
