using dominikz.Domain.Enums.Cookbook;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Cookbook;
using dominikz.Infrastructure;

namespace dominikz.Api.Utils;

internal static class RecipeHelper
{
    public static decimal CalculateFactorByUnit(IngredientVm ingredient, Food food)
    {
        var value = ingredient.Unit switch
        {
            IngredientUnit.Piece => ingredient.Value,
            IngredientUnit.Teaspoon => ingredient.Value * 8,
            IngredientUnit.Tablespoon => ingredient.Value * 15,
            IngredientUnit.G or IngredientUnit.Ml => ingredient.Value,
            IngredientUnit.Kg or IngredientUnit.L => ingredient.Value * 1000,
            _ => throw new ArgumentOutOfRangeException(nameof(ingredient), ingredient, null)
        };

        return Math.Round(value / food.Value, 2, MidpointRounding.AwayFromZero);
    }

    public static void CalculateNutriScore(Recipe recipe)
    {
        var kcal = recipe.Ingredients.Sum(x => x.Factor * x.Food!.CaloriesInKcal) / recipe.Portions;
        var salt = recipe.Ingredients.Sum(x => x.Factor * x.Food!.SaltInG) / recipe.Portions;
        recipe.NutriScore = NutriScoreCalculator.Calculate(new(
            ScoreType.Food,
            NutriScoreCalculator.GetEnergyFromKcal(kcal),
            recipe.Ingredients.Sum(x => x.Factor * x.Food!.SugarInG) / recipe.Portions,
            recipe.Ingredients.Sum(x => x.Factor * x.Food!.FatInG) / recipe.Portions,
            NutriScoreCalculator.GetSodiumFromSalt(salt),
            50,
            recipe.Ingredients.Sum(x => x.Factor * x.Food!.DietaryFiberInG) / recipe.Portions,
            recipe.Ingredients.Sum(x => x.Factor * x.Food!.ProteinInG) / recipe.Portions
        ));
    }
}