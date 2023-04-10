using dominikz.Domain.Contracts;
using dominikz.Domain.Enums.Cookbook;

namespace dominikz.Domain.Filter;

public class RecipeFilter : IFilter
{
    public string? Text { get; init; }
    public RecipeType? Type { get; init; }
    public RecipeFlags? Flags { get; init; }
    public List<Guid> FoodIds { get; init; } = new();

    public IReadOnlyCollection<FilterParam> GetParameter()
    {
        var result = new List<FilterParam>();

        if (string.IsNullOrWhiteSpace(Text) == false)
            result.Add(new(nameof(Text), Text));

        if (Type != null)
            result.Add(new(nameof(Type), Type.ToString()!));

        if (Flags is not null)
            result.Add(new(nameof(Flags), Flags.ToString()!));
        
        if (FoodIds.Count > 0)
            result.AddRange(FoodIds.Select(x => new FilterParam($"{nameof(FoodIds)}[]", x.ToString())));

        return result;
    }
}