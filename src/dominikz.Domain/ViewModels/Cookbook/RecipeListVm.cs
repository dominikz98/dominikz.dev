using dominikz.Domain.Contracts;
using dominikz.Domain.Enums.Cookbook;

namespace dominikz.Domain.ViewModels.Cookbook;

public class RecipeListVm : IHasImageUrl
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public RecipeType Type { get; set; }
    public int Duration { get; set; }
    public List<RecipeFlags> Flags { get; set; } = new();
    public NutriScoreValue NutriScore { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}