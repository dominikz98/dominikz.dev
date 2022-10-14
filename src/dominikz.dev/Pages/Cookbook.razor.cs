using dominikz.dev.Shared.Collection;
using dominikz.dev.Shared.Tables;
using dominikz.kernel.Endpoints;
using dominikz.kernel.ViewModels;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Pages;

public partial class Cookbook
{
    // data
    private List<FoodVM> _foods = new();
    private List<RecipeVM> _recipes = new();

    // order
    private readonly List<string> _orderKeys = new() { nameof(RecipeVM.Title), nameof(RecipeVM.PricePerPortion), nameof(RecipeVM.Portions), nameof(RecipeVM.Duration), nameof(RecipeVM.FoodCount) };
    private OrderInfo _order = new OrderInfo(nameof(RecipeVM.Title), OrderDirection.Ascending);

    // filter
    private string? _search;
    private RecipeCategoryFlags _selectedCategory;
    private List<FoodVM> _selectedFoods = new();

    // view 
    private CollectionView _view;
    private List<ColumnDefinition<RecipeVM>> _recipesColumns = new()
    {
        new (nameof(RecipeVM.Title), x => x.Title),
        new (nameof(RecipeVM.Portions), x => x.Portions),
        new ("Price / Portion", x => x.PricePerPortion) { Formatter = x => $"{x:c2}", Actions = ColumnActionFlags.HIDE_ON_MOBILE },
        new ("Components", x => x.FoodCount) { Actions = ColumnActionFlags.HIDE_ON_MOBILE },
        new (nameof(RecipeVM.Duration), x => x.Duration)
    };

    [Inject]
    protected CookbookEndpoints? Endpoints { get; set; }

    [Inject]
    protected NavigationManager? Navigation { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadFoods();
        await SearchRecipes();
    }

    private async Task LoadFoods()
        => _foods = await Endpoints!.GetFoods();

    private void OnViewChanged(CollectionView view)
        => _view = view;

    private void OnOrderChanged(OrderInfo order)
    {
        _order = order;
        OrderRecipes();
    }

    private async Task OnSearchChanged(string? search)
    {
        _search = search;
        await SearchRecipes();
    }

    private async Task OnCategoryChanged(List<RecipeCategoryFlags> categories)
    {
        _selectedCategory = categories.FirstOrDefault();
        await SearchRecipes();
    }

    private async Task OnFoodsChanged(List<FoodVM> foods)
    {
        _selectedFoods = foods;
        await SearchRecipes();
    }

    private async Task SearchRecipes()
    {
        var selectedFoodIds = _selectedFoods.Select(x => x.Id).ToList();
        var filter = new RecipesFilter
        {
            Category = _selectedCategory,
            Text = _search,
            FoodIds = selectedFoodIds
        };

        _recipes = await Endpoints!.SearchRecipes(filter);
        OrderRecipes();

        StateHasChanged();
    }

    private void OrderRecipes()
    {
        if (_order.Key.Equals(nameof(RecipeVM.Title), StringComparison.OrdinalIgnoreCase))
        {
            if (_order.Direction == OrderDirection.Ascending)
                _recipes = _recipes.OrderBy(x => x.Title).ToList();
            else
                _recipes = _recipes.OrderByDescending(x => x.Title).ToList();
        }
        else if (_order.Key.Equals(nameof(RecipeVM.PricePerPortion), StringComparison.OrdinalIgnoreCase))
        {
            if (_order.Direction == OrderDirection.Ascending)
                _recipes = _recipes.OrderBy(x => x.PricePerPortion).ToList();
            else
                _recipes = _recipes.OrderByDescending(x => x.PricePerPortion).ToList();
        }
        else if (_order.Key.Equals(nameof(RecipeVM.Portions), StringComparison.OrdinalIgnoreCase))
        {
            if (_order.Direction == OrderDirection.Ascending)
                _recipes = _recipes.OrderBy(x => x.Portions).ToList();
            else
                _recipes = _recipes.OrderByDescending(x => x.Portions).ToList();
        }
        else if (_order.Key.Equals(nameof(RecipeVM.Duration), StringComparison.OrdinalIgnoreCase))
        {
            if (_order.Direction == OrderDirection.Ascending)
                _recipes = _recipes.OrderBy(x => x.Duration).ToList();
            else
                _recipes = _recipes.OrderByDescending(x => x.Duration).ToList();
        }
        else if (_order.Key.Equals(nameof(RecipeVM.FoodCount), StringComparison.OrdinalIgnoreCase))
        {
            if (_order.Direction == OrderDirection.Ascending)
                _recipes = _recipes.OrderBy(x => x.FoodCount).ToList();
            else
                _recipes = _recipes.OrderByDescending(x => x.FoodCount).ToList();
        }
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
