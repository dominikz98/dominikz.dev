using dominikz.shared.Contracts;

namespace dominikz.shared.Filter;

public class RecipesFilter : IFilter
{
    public string? Text { get; set; }
    public RecipeCategoryFlags Category { get; set; }
    public List<Guid> FoodIds { get; set; } = new();

    public IReadOnlyCollection<FilterParam> GetParameter()
    {
        var result = new List<FilterParam>();

        if (Text is not null)
            result.Add(new(nameof(Text), Text));

        if (Category != RecipeCategoryFlags.ALL)
            result.Add(new(nameof(Category), Category.ToString()!));

        foreach (var foodId in FoodIds)
            result.Add(new(nameof(FoodIds), foodId.ToString()));

        return result;
    }
}

