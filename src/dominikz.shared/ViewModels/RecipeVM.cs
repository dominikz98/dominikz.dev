using dominikz.shared.Contracts;

namespace dominikz.shared.ViewModels;

public class RecipeVM : IViewModel
{
    public Guid Id { get; init; }
    public FileVM? Image { get; init; }
    public string Title { get; init; } = string.Empty;
    public int Portions { get; init; }
    public decimal PricePerPortion { get; init; }
    public int FoodCount { get; init; }
    public TimeSpan Duration { get; init; }
    public RecipeCategoryFlags Categories { get; init; } = new();
}

public class RecipeDetailVM : RecipeVM
{
    public string Text { get; init; } = string.Empty;
    public List<FoodDetailVM> Foods { get; init; } = new();
}
