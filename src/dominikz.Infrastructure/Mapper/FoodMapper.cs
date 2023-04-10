using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Cookbook;
using dominikz.Infrastructure.Clients.SupermarktCheck;

namespace dominikz.Infrastructure.Mapper;

public static class FoodMapper
{
    public static FoodVm MapToDetailVm(this ProductVm source)
        => new()
        {
            Id = Guid.NewGuid(),
            Name = source.Name,
            SupermarktCheckId = source.Id,
            Price = Math.Round(source.Prices.Average(x => x.Price), 2, MidpointRounding.AwayFromZero),
            CaloriesInKcal = source.NutritionalValues.Where(x => x.Name.Contains("Kalorien")).FirstOrDefault(x => x.Unit == NutritionUnit.Kcal)?.Value ?? 0,
            CarbohydratesInG = source.NutritionalValues.Where(x => x.Name.Contains("Kohlenhydrate")).FirstOrDefault(x => x.Unit == NutritionUnit.G)?.Value ?? 0,
            ProteinInG = source.NutritionalValues.Where(x => x.Name.Contains("Protein")).FirstOrDefault(x => x.Unit == NutritionUnit.G)?.Value ?? 0,
            FatInG = source.NutritionalValues.Where(x => x.Name.Contains("Fett")).FirstOrDefault(x => x.Unit == NutritionUnit.G)?.Value ?? 0,
            DietaryFiberInG = source.NutritionalValues.Where(x => x.Name.Contains("Ballaststoffe")).FirstOrDefault(x => x.Unit == NutritionUnit.G)?.Value ?? 0,
            SugarInG = source.NutritionalValues.Where(x => x.Name.Contains("Zucker")).FirstOrDefault(x => x.Unit == NutritionUnit.G)?.Value ?? 0,
            SaltInG = source.NutritionalValues.Where(x => x.Name.Contains("Salz")).FirstOrDefault(x => x.Unit == NutritionUnit.G)?.Value ?? 0
        };
    
    public static IQueryable<FoodVm> MapToDetailVm(this IQueryable<Food> source)
        => source.Select(x => x.MapToDetailVm());
    
    public static FoodVm MapToDetailVm(this Food source)
        => new()
        {
            Id = source.Id,
            Name = source.Name,
            Unit = source.Unit,
            Value = source.Value,
            Price = Math.Round(source.Snapshots
                .GroupBy(x => x.Timestamp)
                .OrderBy(x => x.Key)
                .Last()
                .Average(x => x.Price), 2, MidpointRounding.AwayFromZero),
            SupermarktCheckId = source.SupermarktCheckId ?? 0,
            CaloriesInKcal = source.CaloriesInKcal,
            CarbohydratesInG = source.CarbohydratesInG,
            ProteinInG = source.ProteinInG,
            FatInG = source.FatInG,
            DietaryFiberInG = source.DietaryFiberInG,
            SaltInG = source.SaltInG,
            SugarInG = source.SugarInG
        };

    public static Food ApplyChanges(this Food source, FoodVm vm)
    {
        source.SupermarktCheckId = vm.SupermarktCheckId;
        source.Name = vm.Name;
        source.Unit = vm.Unit;
        source.Value = vm.Value;
        source.CaloriesInKcal = vm.CaloriesInKcal;
        source.ProteinInG = vm.ProteinInG;
        source.CarbohydratesInG = vm.CarbohydratesInG;
        source.FatInG = vm.FatInG;
        source.DietaryFiberInG = vm.DietaryFiberInG;
        source.SugarInG = vm.SugarInG;
        source.SaltInG = vm.SaltInG;
        return source;
    }
    
    public static IQueryable<FoodListVm> MapToVm(this IQueryable<Food> source)
        => source.Select(x => new FoodListVm
        {
            Id = x.Id,
            Name = x.Name,
            Unit = x.Unit,
            Value = x.Value
        });
}