using dominikz.shared.Contracts;

namespace dominikz.api.Models;

public class Recipe
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string MdText { get; set; } = string.Empty;
    public int Portions { get; set; }
    public TimeSpan Duration { get; set; }
    public RecipeCategoryFlags Categories { get; set; } = new();

    public List<RecipesFoodsMapping> RecipesFoodsMappings { get; set; } = new();
    public StorageFile? File { get; set; }
}

public class RecipesFoodsMapping
{
    public Guid RecipeId { get; set; }
    public decimal Multiplier { get; set; }
    public Guid FoodId { get; set; }

    public Recipe? Recipe { get; set; }
    public Food? Food { get; set; }
}
