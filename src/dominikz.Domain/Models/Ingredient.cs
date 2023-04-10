namespace dominikz.Domain.Models;

public class Ingredient
{
    public Guid RecipeId { get; set; }
    public Guid FoodId { get; set; }
    public decimal Factor { get; set; }
    
    public Recipe? Recipe { get; set; }
    public Food? Food { get; set; }
}