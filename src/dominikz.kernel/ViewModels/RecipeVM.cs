using dominikz.kernel.Contracts;

namespace dominikz.kernel.ViewModels;

public class RecipeVM : IViewModel
{
    public Guid Id { get; set; }
    public FileVM? Image { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Portions { get; set; }
    public decimal PricePerPortion { get; set; }
    public int FoodCount { get; set; }
    public TimeSpan Duration { get; set; }
    public RecipeCategoryFlags Categories { get; set; } = new();
}

public class RecipeDetailVM : RecipeVM
{
    public string Text { get; set; } = string.Empty;
    public List<FoodDetailVM> Foods { get; set; } = new();
}
