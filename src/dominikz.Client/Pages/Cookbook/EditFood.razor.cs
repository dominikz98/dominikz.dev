using dominikz.Client.Api;
using dominikz.Client.Utils;
using dominikz.Domain.ViewModels.Cookbook;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace dominikz.Client.Pages.Cookbook;

public partial class EditFood
{
    [Parameter] public Guid? Id { get; set; }
    [Inject] protected CookbookEndpoints? Endpoints { get; set; }

    private List<FoodWrapper> _foods = new();
    private FoodVm? _vm;
    private EditContext? _editContext;
    private bool _isEnabled;

    protected override async Task OnInitializedAsync()
    {
        _foods = (await Endpoints!.GetFoods())
            .Select(x => new FoodWrapper(x))
            .OrderBy(x => x.Name)
            .ToList();

        _foods.Add(new FoodWrapper(Guid.Empty, "+"));

        if (Id == null)
            return;

        _vm = await Endpoints.GetFoodDraftById(Id.Value);
        if (_vm == null)
            return;

        _editContext = new EditContext(_vm);
        _isEnabled = true;
    }

    private async Task OnFoodChanged(FoodWrapper food)
    {
        if (food == default)
            return;

        if (food.Id == Guid.Empty)
        {
            Id = null;
            _vm = new FoodVm();
            _editContext = new EditContext(_vm);
            _isEnabled = true;
            return;
        }

        Id = food.Id;
        _vm = await Endpoints!.GetFoodDraftById(Id.Value);
        if (_vm == null)
            return;
        
        _editContext = new EditContext(_vm);
        _isEnabled = true;
    }

    private async Task OnSupermarktCheckId(int id)
    {
        if (_vm == null || id == default)
            return;

        _vm.SupermarktCheckId = id;
        var data = await Endpoints!.GetFoodTemplateById(id);
        if (data == null)
            return;

        _vm.CaloriesInKcal = data.CaloriesInKcal;
        _vm.ProteinInG = data.ProteinInG;
        _vm.FatInG = data.FatInG;
        _vm.DietaryFiberInG = data.DietaryFiberInG;
        _vm.CarbohydratesInG = data.CarbohydratesInG;
        _vm.SugarInG = data.SugarInG;
        _vm.SaltInG = data.SaltInG;
        _vm.Price = data.Price;
        
        if (string.IsNullOrWhiteSpace(_vm.Name))
            _vm.Name = data.Name;
    }

    private async Task OnSaveClicked()
    {
        if (_editContext == null
            || _editContext.Validate() == false
            || _vm == null)
            return;

        if (Id == null)
        {
            var added = await Endpoints!.AddFood(_vm);
            if (added == null)
                return;

            Id = added.Id;
            _foods.Add(new FoodWrapper(added));
            _foods = _foods.OrderBy(x => x.Name).ToList();
            return;
        }

        var updated = await Endpoints!.UpdateFood(_vm);
        if (updated == null)
            return;

        var food = _foods.FirstOrDefault(x => x.Id == updated.Id);
        _foods.Remove(food);
        food.Name = updated.Name;
        _foods.Add(food);
        _foods = _foods.OrderBy(x => x.Name).ToList();
    }
}