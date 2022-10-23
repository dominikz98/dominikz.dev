using dominikz.dev.Endpoints;
using dominikz.kernel.Contracts;
using dominikz.kernel.Filter;
using dominikz.kernel.ViewModels;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Pages.Blog;

public partial class Blog
{
    [Inject]
    public BlogEndpoints? Endpoints { get; set; }

    [Inject]
    protected NavigationManager? Navigation { get; set; }

    private string? _search;
    private ArticleCategoryEnum _category;
    private List<ArticleListVM> _articles = new();

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

        _articles = await Endpoints!.Search(filter);
    }

    private string GetInitials(string value)
    {
        var parts = value.Split(" ", StringSplitOptions.RemoveEmptyEntries)
            .Select(x => $"{x[..1].ToUpper()}.")
            .ToList();

        return string.Join("", parts);
    }

    private void NavigateToDetail(Guid articleId)
    {
        var isAvailable = _articles
            .Where(x => x.Id == articleId)
            .Where(x => x.Available)
            .Any();

        if (isAvailable == false)
            return;

        Navigation!.NavigateTo($"/blog/{articleId}");
    }
}
