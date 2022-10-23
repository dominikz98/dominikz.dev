using dominikz.api.Models;
using dominikz.kernel.ViewModels;

namespace dominikz.api.Mapper;

public static class FoodMapper
{
    public static IQueryable<FoodVM> MapToVM(this IQueryable<Food> query)
        => query.Select(food => new FoodVM()
        {
            Id = food.Id,
            Title = food.Title,
            Unit = food.Unit,
            Count = food.Count,
            PricePerCount = food.PricePerCount
        });

    public static IQueryable<FoodDetailVM> MapToVM(this IQueryable<RecipesFoodsMapping> query)
        => query.Select(mapping => new FoodDetailVM()
        {
            Id = mapping.Food!.Id,
            Title = mapping.Food!.Title,
            Unit = mapping.Food!.Unit,
            Count = mapping.Food!.Count,
            PricePerCount = mapping.Food!.PricePerCount,
            Multiplier = mapping!.Multiplier,
            Kilocalories = mapping.Food!.Kilocalories,
            Protein = mapping.Food!.Protein,
            Fat = mapping.Food!.Fat,
            Carbohydrates = mapping.Food!.Carbohydrates,
            ReweUrl = mapping.Food!.ReweUrl,
        });
}
