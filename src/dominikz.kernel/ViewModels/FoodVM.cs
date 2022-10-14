namespace dominikz.kernel.ViewModels;

public class FoodVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public FoodUnitEnum Unit { get; set; }
    public int Count { get; set; }
    public decimal PricePerCount { get; set; }
}

public class FoodDetailVM : FoodVM
{
    public decimal Multiplier { get; set; }
    public decimal Kilocalories { get; set; }
    public decimal Protein { get; set; }
    public decimal Fat { get; set; }
    public decimal Carbohydrates { get; set; }
    public string? ReweUrl { get; set; }
}

public enum FoodUnitEnum
{
    Pieces,
    Grams,
    Liter
}
