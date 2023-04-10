using dominikz.Domain.Enums.Cookbook;

namespace dominikz.Domain.Models;

public class Recipe
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Portions { get; set; }
    public DateTime Created { get; set; }
    public RecipeType Type { get; set; }
    public int PreparationTime { get; set; }
    public int CookingTime { get; set; }
    public RecipeFlags Flags { get; set; }
    public NutriScoreValue NutriScore { get; set; }

    public List<Ingredient> Ingredients { get; set; } = new();
    public List<RecipeStep> Steps { get; set; } = new();
}