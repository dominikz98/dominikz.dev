using dominikz.dev.Endpoints;
using dominikz.shared.ViewModels;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Pages.Cookbook;

public partial class CookbookDetail
{
    [Parameter]
    public Guid? RecipeId { get; set; }

    [Inject]
    protected CookbookEndpoints? Endpoints { get; set; }

    private RecipeDetailVM? _recipe;
    
    protected override async Task OnInitializedAsync()
    {
        if (RecipeId is null)
            return;

        _recipe = await Endpoints!.GetById(RecipeId.Value);
    }
}
