using dominikz.dev.Shared.Tables;
using dominikz.dev.Utils;
using dominikz.kernel.Endpoints;
using dominikz.kernel.ViewModels;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Pages;

public partial class CookbookDetail
{
    [Parameter]
    public Guid? RecipeId { get; set; }

    [Inject]
    protected CookbookEndpoints? Endpoints { get; set; }

    private RecipeDetailVM? _recipe;
    private readonly List<ColumnDefinition<FoodDetailVM>> _foodColumns = new()
    {
        new(nameof(FoodDetailVM.Title), (x) => x.Title),
        new(nameof(FoodDetailVM.Unit), (x) => $"{x.Multiplier * x.Count:0} {EnumConverter.ToString(x.Unit)}"),
        new("x", (x) => $"({x.Multiplier:0.####})") { Actions = ColumnActionFlags.HIDE_ON_MOBILE },
        new("Price", (x) => x.Multiplier * x.PricePerCount) { Formatter = (x) => $"{x:c2}", Actions = ColumnActionFlags.SUM },
        new("Link", (x) => x.ReweUrl) { Actions = ColumnActionFlags.LINK  | ColumnActionFlags.HIDE_ON_MOBILE }
    };

    protected override async Task OnInitializedAsync()
    {
        if (RecipeId is null)
            return;

        _recipe = await Endpoints!.GetById(RecipeId.Value);
    }
}
