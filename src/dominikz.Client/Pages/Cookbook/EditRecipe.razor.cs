using dominikz.Client.Wrapper;
using dominikz.Domain.Enums;
using dominikz.Domain.Enums.Cookbook;
using dominikz.Domain.ViewModels.Cookbook;
using dominikz.Infrastructure.Clients.Api;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace dominikz.Client.Pages.Cookbook;

public partial class EditRecipe
{
    [Parameter] public Guid? RecipeId { get; set; }
    [Inject] internal CookbookEndpoints? Endpoints { get; set; }
    [Inject] internal DownloadEndpoints? DownloadEndpoints { get; set; }
    [Inject] internal NavigationManager? NavManager { get; set; }

    private List<FoodListVm> _foods = new();
    private EditContext? _editContext;
    private readonly EditWithImageWrapper<RecipeVm> _data = new();
    private bool _isEnabled;

    protected override async Task OnInitializedAsync()
    {
        var movieLoaded = await LoadRecipeIfRequired();
        if (movieLoaded == false)
        {
            _data.ViewModel.Id = Guid.NewGuid();
            OnAddIngredientClicked();
            OnAddStepClicked();
        }

        _foods = (await Endpoints!.GetFoods())
            .OrderBy(x => x.Name)
            .ToList();

        _editContext = new EditContext(_data);
        _isEnabled = true;
    }

    private async Task<bool> LoadRecipeIfRequired()
    {
        if (RecipeId == null)
            return false;

        var recipe = await Endpoints!.GetDraftById(RecipeId.Value);
        if (recipe == null)
            return false;

        _data.ViewModel = recipe;

        var file = await DownloadEndpoints!.Image(RecipeId.Value, ImageSizeEnum.Original);
        if (file == null)
            return true;

        _data.Image.Add(file.Value);
        return true;
    }

    private void OnAddStepClicked()
        => _data.ViewModel.Steps.Add(new RecipeStepVm()
        {
            Order = _data.ViewModel.Steps.Count + 1
        });

    private List<IngredientUnit> GetAvailableUnitsByIngredient(Guid foodId)
    {
        var food = _foods.FirstOrDefault(x => x.Id == foodId);
        if (food == null)
            return new List<IngredientUnit>() { IngredientUnit.Piece };

        return food.Unit switch
        {
            FoodUnit.Piece => new List<IngredientUnit> { IngredientUnit.Piece },
            FoodUnit.Ml => new List<IngredientUnit> { IngredientUnit.Ml, IngredientUnit.L, IngredientUnit.Teaspoon, IngredientUnit.Tablespoon },
            FoodUnit.G => new List<IngredientUnit> { IngredientUnit.G, IngredientUnit.Kg, IngredientUnit.Teaspoon, IngredientUnit.Tablespoon },
            _ => throw new ArgumentOutOfRangeException(nameof(foodId), foodId, null)
        };
    }

    private void OnAddIngredientClicked()
    {
        var firstFood = _foods.MinBy(x => x.Name);
        _data.ViewModel.Ingredients.Add(new IngredientVm()
        {
            Id = firstFood?.Id ?? Guid.Empty,
            Value = firstFood?.Value ?? 1,
            Name = firstFood?.Name ?? "Ingredient",
            Unit = IngredientUnit.Piece
        });
    }

    private async Task OnSaveClicked()
    {
        if (_editContext == null || _editContext.Validate() == false)
            return;

        _data.Image[0] = _data.Image[0].CopyTo(_data.ViewModel.Id.ToString());
        var recipe = RecipeId == null
            ? await Endpoints!.Add(_data.ViewModel, _data.Image)
            : await Endpoints!.Update(_data.ViewModel, _data.Image);

        if (recipe == null)
            return;

        NavManager?.NavigateTo($"/cookbook/recipes/{recipe.Id}");
    }
}