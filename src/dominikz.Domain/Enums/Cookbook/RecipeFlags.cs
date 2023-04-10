namespace dominikz.Domain.Enums.Cookbook;

[Flags]
public enum RecipeFlags
{
    Vegetarian = 1,
    Vegan = 2,
    GlutenFree = 4,
    LactoseFree = 8
}