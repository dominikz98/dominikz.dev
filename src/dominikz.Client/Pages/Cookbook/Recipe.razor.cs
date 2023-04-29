using dominikz.Client.Api;
using dominikz.Domain.Enums;
using dominikz.Domain.ViewModels.Cookbook;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Pages.Cookbook;

public partial class Recipe
{
    [Parameter] public Guid RecipeId { get; set; }
    [Inject] internal CookbookEndpoints? Endpoints { get; set; }
    [Inject] internal DownloadEndpoints? DownloadEndpoints { get; set; }
    [Inject] internal NavigationManager? NavManager { get; set; }
    [Inject] protected ICredentialStorage? Credentials { get; set; }

    private RecipeDetailVm _vm = new();
    private bool _hasCreatePermission;

    protected override async Task OnInitializedAsync()
    {
        _vm = await Endpoints!.GetById(RecipeId) ?? new();
        _hasCreatePermission = await Credentials!.HasRight(PermissionFlags.CreateOrUpdate | PermissionFlags.Blog);
    }

    private decimal CalculateSalt()
    {
        var sum = _vm.Ingredients.Sum(x => x.Factor * x.SaltInG);
        return sum <= 0
            ? 0
            : Math.Round(sum / _vm.Portions, 2, MidpointRounding.AwayFromZero);
    }
    
    private decimal CalculateSugar()
    {
        var sum = _vm.Ingredients.Sum(x => x.Factor * x.SugarInG);
        return sum <= 0
            ? 0
            : Math.Round(sum / _vm.Portions, 2, MidpointRounding.AwayFromZero);
    }
    
    private decimal CalculateDietaryFiber()
    {
        var sum = _vm.Ingredients.Sum(x => x.Factor * x.DietaryFiberInG);
        return sum <= 0
            ? 0
            : Math.Round(sum / _vm.Portions, 2, MidpointRounding.AwayFromZero);
    }
    
    private decimal CalculateFat()
    {
        var sum = _vm.Ingredients.Sum(x => x.Factor * x.FatInG);
        return sum <= 0
            ? 0
            : Math.Round(sum / _vm.Portions, 2, MidpointRounding.AwayFromZero);
    }
    
    private decimal CalculateCarbohydrates()
    {
        var sum = _vm.Ingredients.Sum(x => x.Factor * x.CarbohydratesInG);
        return sum <= 0
            ? 0
            : Math.Round(sum / _vm.Portions, 2, MidpointRounding.AwayFromZero);
    }
    
    private decimal CalculateProtein()
    {
        var sum = _vm.Ingredients.Sum(x => x.Factor * x.ProteinInG);
        return sum <= 0
            ? 0
            : Math.Round(sum / _vm.Portions, 2, MidpointRounding.AwayFromZero);
    }
    
    private decimal CalculateCalories()
    {
        var sum = _vm.Ingredients.Sum(x => x.Factor * x.CaloriesInKcal);
        return sum <= 0
            ? 0
            : Math.Round(sum / _vm.Portions, 2, MidpointRounding.AwayFromZero);
    }
}