using System.ComponentModel.DataAnnotations;
using dominikz.Domain.Attributes;
using dominikz.Domain.Enums.Cookbook;

namespace dominikz.Domain.ViewModels.Cookbook;

public class RecipeVm
{
    public Guid Id { get; set; }
    [Required] [MinLength(3)] public string Name { get; set; } = string.Empty;
    [Range(1, int.MaxValue)] public int Portions { get; set; }
    [Range(0, int.MaxValue)] public int PreparationTime { get; set; }
    [Range(0, int.MaxValue)] public int CookingTime { get; set; }
    public List<RecipeFlags> Flags { get; set; } = new();
    [Required] public RecipeType Type { get; set; }
    public NutriScoreValue NutriScore { get; set; }
    [ListNotEmpty] public List<IngredientVm> Ingredients { get; set; } = new();
    [ListNotEmpty] public List<RecipeStepVm> Steps { get; set; } = new();
}