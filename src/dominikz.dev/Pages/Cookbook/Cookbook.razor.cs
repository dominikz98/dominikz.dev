using dominikz.dev.Components;
using dominikz.dev.Definitions;
using dominikz.dev.Endpoints;
using dominikz.shared.Contracts;
using dominikz.shared.Filter;
using dominikz.shared.ViewModels;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Pages.Cookbook;

public partial class Cookbook
{
    // data
    private List<FoodVM> _foods = new();
    private List<RecipeVM> _recipes = new();

    // filter
    private OrderInfo? _order;
    private string? _search;
    private CollectionView _view;
    private List<RecipeCategoryFlags> _selectedCategories = new();
    private List<Guid> _selectedFoodIds = new();

    [Inject]
    protected CookbookEndpoints? Endpoints { get; set; }

    [Inject]
    protected NavigationManager? Navigation { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _order = new OrderInfo(nameof(RecipeVM.Title), OrderDirection.Ascending);
        await SearchRecipes();
        await LoadFoods();
    }

    private async Task LoadFoods()
        => _foods = await Endpoints!.GetFoods();

    private async Task OnSearchChanged(string? search)
    {
        _search = search;
        await SearchRecipes();
    }

    private async Task OnCategoriesChanged(List<RecipeCategoryFlags> categories)
    {
        _selectedCategories = categories;
        await SearchRecipes();
    }

    private async Task OnFoodsChanged(List<FoodVM> foods)
    {
        _selectedFoodIds = foods.Select(x => x.Id).ToList();
        await SearchRecipes();
    }

    private void OnOrderChanged(OrderInfo order)
    {
        _order = order;
        OrderRecipes();
    }

    private async Task SearchRecipes()
    {
        var filter = new RecipesFilter
        {
            Category = (RecipeCategoryFlags)_selectedCategories.Sum(x => (int)x),
            Text = _search,
            FoodIds = _selectedFoodIds
        };

        _recipes = await Endpoints!.SearchRecipes(filter);
        OrderRecipes();
    }

    private void OrderRecipes()
    {
        if (_order is null)
            return;

        _recipes = _recipes.OrderByKey(_order).ToList();
    }

    private void NavigateToDetail(Guid recipeId)
    {
        var isAvailable = _recipes
            .Where(x => x.Id == recipeId)
            .Any();

        if (isAvailable == false)
            return;

        Navigation!.NavigateTo($"/cookbook/{recipeId}");
    }
}
