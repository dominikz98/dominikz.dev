namespace dominikz.kernel.ViewModels;

public class RecipeVM
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Portions { get; set; }
    public decimal PricePerPortion { get; set; }
    public int FoodCount { get; set; }
    public TimeSpan Duration { get; set; }
    public List<RecipeCategoryFlags> Categories { get; set; } = new();
}

public class RecipeDetailVM : RecipeVM
{
    public string HtmlText { get; set; } = string.Empty;
    public List<FoodDetailVM> Foods { get; set; } = new();
}

[Flags]
public enum RecipeCategoryFlags
{
    ALL = 0,
    Vegetarian = 1,
    Starter = 4,
    Dish = 8,
    Dessert = 16
}
