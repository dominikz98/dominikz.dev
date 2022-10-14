using dominikz.kernel.Endpoints;
using dominikz.kernel.ViewModels;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Pages;

public partial class Blog
{
    [Inject]
    public BlogEndpoints? Endpoints { get; set; }

    [Inject]
    protected NavigationManager? Navigation { get; set; }

    private string? _search;
    private ArticleCategoryEnum _category;

    private List<ArticleListVM> _filteredArticles = new();
    private List<ArticleListVM> _featureArticles = new();

    protected override async Task OnInitializedAsync()
        => await SearchArticles();

    private async Task OnSearchChanged(string? search)
    {
        _search = search;
        await SearchArticles();
    }

    private async Task OnCategoryChanged(List<ArticleCategoryEnum> category)
    {
        _category = category.FirstOrDefault();
        await SearchArticles();
    }

    private async Task SearchArticles()
    {
        var filter = new ArticleFilter()
        {
            Category = _category,
            Text = _search
        };

        var all = await Endpoints!.Search(filter);
        _featureArticles = all.Where(x => x.Featured).ToList();
        _filteredArticles = all.Except(_featureArticles).ToList();
    }

    private void NavigateToDetail(Guid articleId)
    {
        var isAvailable = _featureArticles
            .Union(_filteredArticles)
            .Where(x => x.Id == articleId)
            .Where(x => x.Available)
            .Any();

        if (isAvailable == false)
            return;

        Navigation!.NavigateTo($"/blog/{articleId}");
    }
}