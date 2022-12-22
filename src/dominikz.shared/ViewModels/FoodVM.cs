using dominikz.shared.Contracts;

namespace dominikz.shared.ViewModels;

public class FoodVM : IViewModel
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public FoodUnitEnum Unit { get; set; }
    public int Count { get; set; }
    public decimal PricePerCount { get; set; }
}

public class FoodDetailVM
{
    public Guid Id { get; set; }
    public string Title { get; init; } = string.Empty;
    public FoodUnitEnum Unit { get; init; }
    public int Count { get; init; }
    public decimal PricePerCount { get; init; }
    public decimal Multiplier { get; init; }
    public decimal Kilocalories { get; init; }
    public decimal Protein { get; init; }
    public decimal Fat { get; init; }
    public decimal Carbohydrates { get; init; }
    public string? ReweUrl { get; init; }
}
