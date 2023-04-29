using dominikz.Client.Api;
using dominikz.Client.Components;
using dominikz.Client.Components.Chips;
using dominikz.Client.Components.Toast;
using dominikz.Client.Extensions;
using dominikz.Client.Tables;
using dominikz.Client.Utils;
using dominikz.Domain.Enums;
using dominikz.Domain.Enums.Movies;
using dominikz.Domain.Filter;
using dominikz.Domain.ViewModels.Movies;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace dominikz.Client.Pages.Movies;

public partial class Movies : ComponentBase
{
    [Inject] protected MovieEndpoints? Endpoints { get; set; }
    [Inject] protected NavigationManager? NavManager { get; set; }
    [Inject] protected BrowserService? Browser { get; set; }
    [Inject] protected ToastService? Toast { get; set; }
    [Inject] protected ICredentialStorage? Credentials { get; set; }

    private List<MoviePreviewVm> _previews = new();
    private readonly List<MovieVm> _movies = new();
    private bool _hasCreatePermission;

    private bool _isTableView;
    private TextBox? _searchBox;
    private ChipSelect<MovieGenresFlags>? _movieGenreSelect;
    private const int LoadingPackageSize = 50;
    private readonly List<CancellationTokenSource> _cancellationSources = new();
    private bool _streamingMode;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        _streamingMode = await Browser!.IsStreamingModeEnabled();
    }

    protected override async Task OnInitializedAsync()
    {
        _hasCreatePermission = await Credentials!.HasRight(PermissionFlags.CreateOrUpdate | PermissionFlags.Movies);
        _previews = await Endpoints!.GetPreview();

        var filter = CreateFilter();
        _searchBox?.SetValue(filter.Text);
        _movieGenreSelect?.Select(filter.Genres);

        var init = NavManager!.TrackQuery(SearchMovies);
        if (init)
            await SearchMovies();
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

        if (string.IsNullOrWhiteSpace(curl))
        {
            Toast!.Show("Error creating CURL", ToastLevel.Error);
            return;
        }

        await Browser!.CopyToClipboard(curl);
        Toast!.Show("CURL stored in clipboard", ToastLevel.Success);
    }

    private MoviesFilter CreateFilter()
        => new()
        {
            Text = NavManager!.GetQueryParamByKey(QueryNames.Movies.Search),
            Genres = NavManager!.GetQueryParamByKey<MovieGenresFlags>(QueryNames.Movies.Genre)
        };

    private async Task SearchMovies()
    {
        var filter = CreateFilter();
        var count = await Endpoints!.SearchCount(filter);
        _movies.Clear();

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
            var movies = await Endpoints!.Search(filter, cancellationSource.Token);
            _movies.AddRange(movies);
            StateHasChanged();
        }

        _cancellationSources.Remove(cancellationSource);
    }

    private void NavigateToMovie(Guid movieId)
        => NavManager!.NavigateTo($"/movies/{movieId}");
}