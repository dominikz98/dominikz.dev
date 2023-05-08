using dominikz.Client.Api;
using dominikz.Client.Components;
using dominikz.Client.Components.Chips;
using dominikz.Client.Components.Toast;
using dominikz.Client.Extensions;
using dominikz.Client.Tables;
using dominikz.Client.Utils;
using dominikz.Domain.Enums;
using dominikz.Domain.Enums.Cookbook;
using dominikz.Domain.Filter;
using dominikz.Domain.ViewModels.Cookbook;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Pages.Cookbook;

public partial class Cookbook
{
    [Inject] internal CookbookEndpoints? Endpoints { get; set; }
    [Inject] protected NavigationManager? NavManager { get; set; }
    [Inject] internal BrowserService? Browser { get; set; }
    [Inject] protected ToastService? Toast { get; set; }
    [Inject] protected ICredentialStorage? Credentials { get; set; }

    private List<RecipeListVm> _recipes = new();
    private bool _isTableView;
    private bool _hasCreatePermission;

    private TextBox? _searchBox;
    private ChipSelect<RecipeType>? _typeSelect;
    private ChipSelect<RecipeFlags>? _flagsSelect;

    protected override async Task OnInitializedAsync()
    {
        _hasCreatePermission = await Credentials!.HasRight(PermissionFlags.CreateOrUpdate | PermissionFlags.Blog);

        var filter = CreateFilter();
        _searchBox?.SetValue(filter.Text);
        _typeSelect?.Select(filter.Type);
        _flagsSelect?.Select(filter.Flags);

        var init = NavManager!.TrackQuery(SearchRecipes);
        if (init)
            await SearchRecipes();
    }

    private async Task SearchRecipes()
    {
        var filter = CreateFilter();
        _recipes = await Endpoints!.Search(filter);
    }

    private async Task OnCopyLinkClicked()
    {
        await Browser!.CopyToClipboard(NavManager!.Uri);
        Toast!.Show("Link stored in clipboard", ToastLevel.Success);
    }

    private async Task OnCreateCURLClicked()
    {
        var filter = CreateFilter();
        var curl = Endpoints!.CurlSearch(filter);
        await Browser!.CopyToClipboard(curl);
        Toast!.Show("CURL stored in clipboard", ToastLevel.Success);
    }

    private RecipeFilter CreateFilter()
        => new()
        {
            Type = NavManager!.GetQueryParamByKey<RecipeType>(QueryNames.Cookbook.Type, false),
            Flags = NavManager!.GetQueryParamByKey<RecipeFlags>(QueryNames.Cookbook.Flags, false),
            Text = NavManager!.GetQueryParamByKey(QueryNames.Cookbook.Search)
        };

    private string? NavigateToDetail(Guid articleId)
    {
        var article = _recipes.FirstOrDefault(x => x.Id == articleId);
        if (article is null)
            return null;

        return $"/cookbook/recipes/{article.Id}";
    }
}