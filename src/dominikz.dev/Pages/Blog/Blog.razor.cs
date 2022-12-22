using dominikz.dev.Components;
using dominikz.dev.Components.Chips;
using dominikz.dev.Components.Toast;
using dominikz.dev.Endpoints;
using dominikz.dev.Models;
using dominikz.dev.Utils;
using dominikz.shared.Contracts;
using dominikz.shared.Filter;
using dominikz.shared.ViewModels;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Pages.Blog;

public partial class Blog
{
    [Inject] public BlogEndpoints? Endpoints { get; set; }

    [Inject] protected NavigationManager? NavManager { get; set; }

    [Inject] protected BrowserService? Browser { get; set; }

    [Inject] protected ToastService? Toast { get; set; }

    private List<ArticleListVm> _articles = new();
    private int _view = (int)CollectionView.Grid;

    // refs
    private Searchbox? _searchbox;
    private ChipSelect<ArticleCategoryEnum>? _categorySelect;
    private ChipSelect<ArticleSource>? _sourceSelect;

    protected override async Task OnInitializedAsync()
    {
        NavManager!.LocationChanged += async (_, _) => await SearchArticles();
        await SearchArticles();
    }
  
    private async Task SearchArticles()
    {
        var filter = new ArticleFilter()
        {
            Category = _categorySelect?.Selected,
            Sources = _sourceSelect?.Selected,
            Text = _searchbox?.Value
        };

        _articles = await Endpoints!.Search(filter);
        StateHasChanged();
    }

    private async Task OnRssFeedClicked()
    {
        var url = Endpoints!.GetRssFeedUrl();
        await Browser!.CopyToClipboard(url);
        Toast!.Show("Rss-Feed stored in clipboard", ToastLevel.Success);
    }

    private async Task OnCopyLinkClicked()
    {
        await Browser!.CopyToClipboard(NavManager!.Uri);
        Toast!.Show("Link stored in clipboard", ToastLevel.Success);
    }

    private async Task OnCreateCURLClicked()
    {
        var curl = $"curl {NavManager!.Uri}";
        await Browser!.CopyToClipboard(curl);
        Toast!.Show("CURL stored in clipboard", ToastLevel.Success);
    }
    
    private void NavigateToDetail(Guid articleId)
    {
        var article = _articles.FirstOrDefault(x => x.Id == articleId);
        if (article is null)
            return;

        if (article.Source != ArticleSource.Dz)
        {
            NavManager!.NavigateTo(article.Path);
            return;
        }

        NavManager!.NavigateTo($"/blog/{article.Id}");
    }
}