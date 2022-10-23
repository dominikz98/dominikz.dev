using dominikz.api.Models;
using dominikz.api.Utils;
using dominikz.kernel.ViewModels;

namespace dominikz.api.Mapper;

public static class RecipeMapper
{
    public static IQueryable<RecipeVM> MapToVM(this IQueryable<Recipe> query)
        => query.Select(recipe => new RecipeVM()
        {
            Id = recipe.Id,
            Image = recipe.File!.MapToVM(),
            Title = recipe.Title,
            Portions = recipe.Portions,
            PricePerPortion = recipe.RecipesFoodsMappings.Sum(y => y.Multiplier * y.Food!.PricePerCount) / recipe.Portions,
            FoodCount = recipe.RecipesFoodsMappings.Count,
            Duration = recipe.Duration,
            Categories = recipe.Categories
        });

    public static RecipeDetailVM MapToDetailVM(this Recipe recipe)
        => new()
        {
            Id = recipe.Id,
            Image = recipe.File!.MapToVM(),
            Title = recipe.Title,
            Portions = recipe.Portions,
            PricePerPortion = recipe.RecipesFoodsMappings.Sum(y => y.Multiplier * y.Food!.PricePerCount) / recipe.Portions,
            FoodCount = recipe.RecipesFoodsMappings.Count,
            Duration = recipe.Duration,
            Categories = recipe.Categories,
            Text = recipe.MDText.ToHtml5(),
            Foods = recipe.RecipesFoodsMappings.AsQueryable().MapToVM().ToList()
        };
}
