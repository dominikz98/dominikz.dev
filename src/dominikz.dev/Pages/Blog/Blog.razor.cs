using dominikz.dev.Components;
using dominikz.dev.Definitions;
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
    private CollectionView _view;

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

    private void OnOrderChanged(OrderInfo order)
        => _articles = _articles.OrderByKey(order).ToList();

    private async Task SearchArticles()
    {
        var filter = new ArticleFilter()
        {
            Category = _category,
            Text = _search
        };

        _articles = await Endpoints!.Search(filter);
    }

    private static string GetInitials(string value)
    {
        var parts = value.Split(" ", StringSplitOptions.RemoveEmptyEntries)
            .Select(x => $"{x[..1].ToUpper()}.")
            .ToList();

        return string.Join("", parts);
    }

    private void NavigateToDetail(Guid articleId)
    {
        var article = _articles
            .Where(x => x.Id == articleId)
            .FirstOrDefault();

        if (article is null)
            return;

        if (article.Path.StartsWith('~'))
            Navigation!.NavigateTo(article.Path[1..].ToString());
        else
            Navigation!.NavigateTo(article.Path.ToString());
    }
}
