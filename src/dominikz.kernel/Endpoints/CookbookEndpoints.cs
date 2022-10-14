using dominikz.kernel.ViewModels;

namespace dominikz.kernel.Endpoints;

public class CookbookEndpoints
{
    private readonly ApiClient _client;
    private static readonly string _endpoint = "cookbook";

    public CookbookEndpoints(ApiClient client)
    {
        _client = client;
    }

    public async Task<List<FoodVM>> GetFoods(CancellationToken cancellationToken = default)
        => await _client.Get<FoodVM>($"{_endpoint}/foods", cancellationToken);

    public async Task<RecipeDetailVM?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await _client.Get<RecipeDetailVM?>($"{_endpoint}/recipes", id, cancellationToken);
    public async Task<List<RecipeVM>> SearchRecipes(RecipesFilter filter, CancellationToken cancellationToken = default)
        => await _client.Get<RecipeVM>($"{_endpoint}/recipes/search", filter, cancellationToken);
}

public class RecipesFilter : IFilter
{
    public string? Text { get; set; }
    public RecipeCategoryFlags Category { get; set; }
    public List<Guid> FoodIds { get; set; } = new();

    public static RecipesFilter Default => new();

    public IReadOnlyCollection<FilterParam> GetParameter()
    {
        var result = new List<FilterParam>();

        if (Text is not null)
            result.Add(new(nameof(Text), Text));

        if (Category != RecipeCategoryFlags.ALL)
            result.Add(new(nameof(Category), Category.ToString()!));

        foreach (var foodId in FoodIds)
            result.Add(new (nameof(FoodIds), foodId.ToString()));

        return result;
    }
}
