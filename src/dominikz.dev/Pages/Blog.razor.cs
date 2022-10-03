using dominikz.kernel.Endpoints;
using dominikz.kernel.ViewModels;
using MatBlazor;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Pages;

public partial class Blog
{
    [Inject]
    public ArticlesEndpoints? Endpoints { get; set; }

    [Inject]
    protected NavigationManager? Navigation { get; set; }

    private string? _search;
    private MatChip? _category;
    private List<string> _categories = new();
    private List<ArticleVM> _filteredArticles = new();
    private List<ArticleVM> _featureArticles = new();

    protected override async Task OnInitializedAsync()
        => await FilterArticles(ArticleFilter.Default);

    private async Task FilterArticles(ArticleFilter? filter)
    {
        _categories = await Endpoints!.GetAllCategories();

        filter ??= ArticleFilter.Default;
        var all = await Endpoints!.Search(filter);
        _featureArticles = all.Where(x => x.Featured).ToList();
        _filteredArticles = all.Except(_featureArticles).ToList();
    }

    private async Task OnFilterChanged()
    {
        var filter = new ArticleFilter()
        {
            Category = _category?.Label,
            Text = _search
        };
        await FilterArticles(filter);
    }

    private void NavigateToDetail(Guid articleId)
        => Navigation!.NavigateTo($"/blog/{articleId}");
}