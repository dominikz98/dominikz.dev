using dominikz.shared.Filter;
using dominikz.shared.ViewModels;

namespace dominikz.dev.Endpoints;

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

