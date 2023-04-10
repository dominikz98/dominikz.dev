namespace dominikz.Domain.Models;

public class RecipeStep
{
    public Guid RecipeId { get; set; }
    public int Order { get; set; }
    public string Text { get; set; } = string.Empty;
}