using dominikz.Client.Components;
using dominikz.Client.Components.Chips;
using dominikz.Client.Components.Toast;
using dominikz.Client.Extensions;
using dominikz.Client.Tables;
using dominikz.Client.Utils;
using dominikz.Domain.Enums;
using dominikz.Domain.Enums.Blog;
using dominikz.Domain.Filter;
using dominikz.Domain.ViewModels.Blog;
using dominikz.Infrastructure.Clients.Api;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Pages.Blog;

public partial class Blog
{
    [Inject] internal BlogEndpoints? Endpoints { get; set; }
    [Inject] protected NavigationManager? NavManager { get; set; }
    [Inject] internal BrowserService? Browser { get; set; }
    [Inject] protected ToastService? Toast { get; set; }
    [Inject] protected ICredentialStorage? Credentials { get; set; }

    private readonly List<ArticleVm> _articles = new();
    private bool _isTableView;
    private bool _hasCreatePermission;

    private TextBox? _searchBox;
    private ChipSelect<ArticleCategoryEnum>? _categorySelect;
    private ChipSelect<ArticleSourceEnum>? _sourceSelect;
    private const int LoadingPackageSize = 100;
    private readonly List<CancellationTokenSource> _cancellationSources = new();

    protected override async Task OnInitializedAsync()
    {
        _hasCreatePermission = await Credentials!.HasRight(PermissionFlags.CreateOrUpdate | PermissionFlags.Blog);

        var filter = CreateFilter();
        _searchBox?.SetValue(filter.Text);
        _categorySelect?.Select(filter.Category);
        _sourceSelect?.Select(filter.Source);

        var init = NavManager!.TrackQuery(SearchArticles);
        if (init)
            await SearchArticles();
    }

    private async Task SearchArticles()
    {
        var filter = CreateFilter();
        var count = await Endpoints!.SearchCount(filter);
        _articles.Clear();
        StateHasChanged();
        
        foreach (var toCancel in _cancellationSources)
            toCancel.Cancel();

        var cancellationSource = new CancellationTokenSource();
        _cancellationSources.Add(cancellationSource);
        
        for (var i = 0; i < count; i += LoadingPackageSize)
        {
            if (cancellationSource.IsCancellationRequested)
                break;

            filter.Start = i;
            filter.Count = Math.Min(LoadingPackageSize, count - i);
            var articles = await Endpoints!.Search(filter, cancellationSource.Token);
            _articles.AddRange(articles);
            StateHasChanged();
        }

        _cancellationSources.Remove(cancellationSource);
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