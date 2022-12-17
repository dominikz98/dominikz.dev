using dominikz.shared.Contracts;

namespace dominikz.api.Models;

public class Food
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public FoodUnitEnum Unit { get; set; }
    public int Count { get; set; }
    public decimal PricePerCount { get; set; }
    public decimal Kilocalories { get; set; }
    public decimal Protein { get; set; }
    public decimal Fat { get; set; }
    public decimal Carbohydrates { get; set; }
    public string? ReweUrl { get; set; }

    public virtual ICollection<RecipesFoodsMapping> RecipesFoodsMappings { get; set; } = new HashSet<RecipesFoodsMapping>();
}
