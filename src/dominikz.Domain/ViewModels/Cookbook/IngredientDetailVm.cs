using dominikz.Domain.Enums.Cookbook;

namespace dominikz.Domain.ViewModels.Cookbook;

public class IngredientDetailVm
{
    public Guid Id { get; set; }
    public FoodUnit Unit { get; set; }
    public decimal Factor { get; set; } = 1;
    public decimal Value { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal CaloriesInKcal { get; set; }
    public decimal ProteinInG { get; set; }
    public decimal CarbohydratesInG { get; set; }
    public decimal FatInG { get; set; }
    public decimal DietaryFiberInG { get; set; }
    public decimal SugarInG { get; set; }
    public decimal SaltInG { get; set; }
}