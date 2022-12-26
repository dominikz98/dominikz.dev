using dominikz.shared.Filter;
using dominikz.shared.ViewModels;

namespace dominikz.dev.Endpoints;

public class CookbookEndpoints
{
    private readonly ApiClient _client;
    private const string Endpoint = "cookbook";

    public CookbookEndpoints(ApiClient client)
    {
        _client = client;
    }

    public async Task<List<FoodVM>> GetFoods(CancellationToken cancellationToken = default)
        => await _client.Get<FoodVM>($"{Endpoint}/foods", cancellationToken);

    public async Task<RecipeDetailVM?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await _client.Get<RecipeDetailVM?>($"{Endpoint}/recipes", id, cancellationToken);
    public async Task<List<RecipeVM>> SearchRecipes(RecipesFilter filter, CancellationToken cancellationToken = default)
        => await _client.Get<RecipeVM>($"{Endpoint}/recipes/search", filter, cancellationToken);
}

