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

    private const string QuerySearch = "search";
    private const string QueryCategory = "category";
    private const string QuerySource = "source";

    private Searchbox? _searchbox;
    private ChipSelect<ArticleCategoryEnum>? _categorySelect;
    private ChipSelect<ArticleSourceEnum>? _sourceSelect;

    protected override async Task OnInitializedAsync()
    {
        NavManager!.LocationChanged += async (_, _) => await SearchArticles();
        await SearchArticles();

        var search = NavManager.GetQueryParamByKey(QuerySearch);
        _searchbox?.SetValue(search);

        var category = NavManager.GetQueryParamByKey<ArticleCategoryEnum>(QueryCategory);
        _categorySelect?.Select(category);

        var source = NavManager.GetQueryParamByKey<ArticleSourceEnum>(QuerySource);
        _sourceSelect?.Select(source);
    }

    private async Task SearchArticles()
    {
        var filter = CreateFilter();
        _articles = await Endpoints!.Search(filter);
        StateHasChanged();
    }

    private ArticleFilter CreateFilter()
        => new()
        {
            Category = NavManager!.GetQueryParamByKey<ArticleCategoryEnum>(QueryCategory),
            Sources = NavManager!.GetQueryParamByKey<ArticleSourceEnum>(QuerySource),
            Text = NavManager!.GetQueryParamByKey(QuerySearch)
        };

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
        var filter = CreateFilter();
        var curl = Endpoints!.CurlSearch(filter);
        await Browser!.CopyToClipboard(curl);
        Toast!.Show("CURL stored in clipboard", ToastLevel.Success);
    }

    private void NavigateToDetail(Guid articleId)
    {
        var article = _articles.FirstOrDefault(x => x.Id == articleId);
        if (article is null)
            return;

        if (article.SourceEnum != ArticleSourceEnum.Dz)
        {
            NavManager!.NavigateTo(article.Path);
            return;
        }

        NavManager!.NavigateTo($"/blog/{article.Id}");
    }
}