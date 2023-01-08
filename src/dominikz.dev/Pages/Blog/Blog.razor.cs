using dominikz.dev.Components;
using dominikz.dev.Components.Chips;
using dominikz.dev.Components.Toast;
using dominikz.dev.Definitions;
using dominikz.dev.Endpoints;
using dominikz.dev.Extensions;
using dominikz.dev.Utils;
using dominikz.shared.Enums;
using dominikz.shared.Filter;
using dominikz.shared.ViewModels.Blog;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Pages.Blog;

public partial class Blog
{
    [Inject] internal BlogEndpoints? Endpoints { get; set; }
    [Inject] protected NavigationManager? NavManager { get; set; }
    [Inject] internal BrowserService? Browser { get; set; }
    [Inject] protected ToastService? Toast { get; set; }
    [Inject] protected CredentialStorage? Credentials { get; set; }

    private List<ArticleVm> _articles = new();
    private bool _isTableView;
    private bool _hasCreatePermission;

    private TextBox? _searchbox;
    private ChipSelect<ArticleCategoryEnum>? _categorySelect;
    private ChipSelect<ArticleSourceEnum>? _sourceSelect;

    protected override async Task OnInitializedAsync()
    {
        _hasCreatePermission = await Credentials!.HasRight(PermissionFlags.CreateOrUpdate | PermissionFlags.Blog);
        NavManager!.LocationChanged += async (_, _) => await SearchArticles();
        await SearchArticles();

        var filter = CreateFilter();
        _searchbox?.SetValue(filter.Text);
        _categorySelect?.Select(filter.Category);
        _sourceSelect?.Select(filter.Source);
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
            Category = NavManager!.GetQueryParamByKey<ArticleCategoryEnum>(QueryNames.Blog.Category, false),
            Source = NavManager!.GetQueryParamByKey<ArticleSourceEnum>(QueryNames.Blog.Source, false),
            Text = NavManager!.GetQueryParamByKey(QueryNames.Blog.Search)
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

        if (article.Source != ArticleSourceEnum.Dz)
        {
            NavManager!.NavigateTo(article.Path);
            return;
        }

        NavManager!.NavigateTo($"/blog/{article.Id}");
    }
}