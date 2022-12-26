using dominikz.shared.Contracts;

namespace dominikz.shared.Filter;

public class RecipesFilter : IFilter
{
    public string? Text { get; init; }
    public RecipeCategoryFlags? Category { get; init; }
    public List<Guid> FoodIds { get; init; } = new();

    public IReadOnlyCollection<FilterParam> GetParameter()
    {
        var result = new List<FilterParam>();

        if (Text is not null)
            result.Add(new(nameof(Text), Text));

        if (Category is not null && Category != RecipeCategoryFlags.ALL)
            result.Add(new(nameof(Category), Category.ToString()!));

        foreach (var foodId in FoodIds)
            result.Add(new(nameof(FoodIds), foodId.ToString()));

        return result;
    }
}

