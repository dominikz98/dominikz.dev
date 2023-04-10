using dominikz.Domain.Enums.Cookbook;
using dominikz.Domain.Extensions;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Cookbook;

namespace dominikz.Infrastructure.Mapper;

public static class RecipeMapper
{
    public static IQueryable<RecipeDetailVm> MapToDetailVm(this IQueryable<Recipe> source)
        => source.Select(x => new RecipeDetailVm()
        {
            Id = x.Id,
            Name = x.Name,
            Type = x.Type,
            Portions = x.Portions,
            CookingTime = x.CookingTime,
            PreparationTime = x.PreparationTime,
            NutriScore = x.NutriScore,
            Flags = x.Flags.GetFlags(),
            Ingredients = x.Ingredients.MapToDetailVm().ToList(),
            Steps = x.Steps.MapToVm().ToList()
        });

    public static IQueryable<RecipeVm> MapToVm(this IQueryable<Recipe> source)
        => source.Select(x => new RecipeVm()
        {
            Id = x.Id,
            Name = x.Name,
            Type = x.Type,
            Portions = x.Portions,
            CookingTime = x.CookingTime,
            PreparationTime = x.PreparationTime,
            NutriScore = x.NutriScore,
            Flags = x.Flags.GetFlags(),
            Ingredients = x.Ingredients.MapToVm().ToList(),
            Steps = x.Steps.MapToVm().ToList()
        });

    public static RecipeListVm MapToListVm(this Recipe source)
        => new()
        {
            Id = source.Id,
            Name = source.Name,
            Flags = source.Flags.GetFlags(),
            Duration = source.PreparationTime + source.CookingTime,
            Type = source.Type,
            NutriScore = source.NutriScore
        };

    private static IEnumerable<IngredientVm> MapToVm(this IEnumerable<Ingredient> source)
        => source.Select(x => new IngredientVm()
        {
            Name = x.Food!.Name,
            Value = x.Factor * x.Food.Value,
            Id = x.FoodId,
            Unit = x.Food.Unit switch
            {
                FoodUnit.Piece => IngredientUnit.Piece,
                FoodUnit.Ml => IngredientUnit.Ml,
                FoodUnit.G => IngredientUnit.G,
                _ => throw new ArgumentOutOfRangeException()
            }
        });

    private static IEnumerable<IngredientDetailVm> MapToDetailVm(this IEnumerable<Ingredient> source)
        => source.Select(x => new IngredientDetailVm()
        {
            Id = x.FoodId,
            Name = x.Food!.Name,
            Unit = x.Food.Unit,
            Value = x.Food.Value,
            CaloriesInKcal = x.Food.CaloriesInKcal,
            CarbohydratesInG = x.Food.CarbohydratesInG,
            FatInG = x.Food.FatInG,
            ProteinInG = x.Food.ProteinInG,
            SaltInG = x.Food.SaltInG,
            SugarInG = x.Food.SugarInG,
            DietaryFiberInG = x.Food.DietaryFiberInG,
            Factor = x.Factor
        });

    private static IEnumerable<RecipeStepVm> MapToVm(this IEnumerable<RecipeStep> source)
        => source.Select(x => new RecipeStepVm()
        {
            Order = x.Order,
            Text = x.Text
        });

    public static Recipe ApplyChanges(this Recipe original, RecipeVm vm)
    {
        original.Id = vm.Id;
        original.Name = vm.Name;
        original.Type = vm.Type;
        original.Portions = vm.Portions;
        original.CookingTime = vm.CookingTime;
        original.PreparationTime = vm.PreparationTime;
        original.Flags = (RecipeFlags)vm.Flags.Sum(x => (int)x);
        return original;
    }

    public static RecipeStep ApplyChanges(this RecipeStep original, RecipeStepVm vm, Guid recipeId)
    {
        original.Order = vm.Order;
        original.RecipeId = recipeId;
        original.Text = vm.Text;
        return original;
    }

    public static Ingredient ApplyChanges(this Ingredient original, IngredientVm vm, Guid recipeId)
    {
        original.FoodId = vm.Id;
        original.RecipeId = recipeId;
        original.Factor = vm.Value;
        return original;
    }
}