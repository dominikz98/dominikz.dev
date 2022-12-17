using dominikz.api.Models;
using dominikz.api.Utils;
using dominikz.shared.ViewModels;

namespace dominikz.api.Mapper;

public static class RecipeMapper
{
    public static IQueryable<RecipeVM> MapToVm(this IQueryable<Recipe> query)
        => query.Select(recipe => new RecipeVM()
        {
            Id = recipe.Id,
            Image = recipe.File!.MapToVm(),
            Title = recipe.Title,
            Portions = recipe.Portions,
            PricePerPortion = recipe.RecipesFoodsMappings.Sum(y => y.Multiplier * y.Food!.PricePerCount) / recipe.Portions,
            FoodCount = recipe.RecipesFoodsMappings.Count,
            Duration = recipe.Duration,
            Categories = recipe.Categories
        });

    public static RecipeDetailVM MapToDetailVm(this Recipe recipe)
        => new()
        {
            Id = recipe.Id,
            Image = recipe.File!.MapToVm(),
            Title = recipe.Title,
            Portions = recipe.Portions,
            PricePerPortion = recipe.RecipesFoodsMappings.Sum(y => y.Multiplier * y.Food!.PricePerCount) / recipe.Portions,
            FoodCount = recipe.RecipesFoodsMappings.Count,
            Duration = recipe.Duration,
            Categories = recipe.Categories,
            Text = recipe.MdText.ToHtml5(),
            Foods = recipe.RecipesFoodsMappings.AsQueryable().MapToVm().ToList()
        };
}
