using dominikz.Domain.Contracts;
using dominikz.Domain.Enums.Cookbook;

namespace dominikz.Domain.ViewModels.Cookbook;

public class RecipeDetailVm : IHasImageUrl
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Portions { get; set; }
    public int PreparationTime { get; set; }
    public int CookingTime { get; set; }
    public List<RecipeFlags> Flags { get; set; } = new();
    public RecipeType Type { get; set; }
    public NutriScoreValue NutriScore { get; set; }
    public List<IngredientDetailVm> Ingredients { get; set; } = new();
    public List<PriceSnapshotVm> PriceSnapshots { get; set; } = new();
    public List<RecipeStepVm> Steps { get; set; } = new();
    public string ImageUrl { get; set; } = string.Empty;
}