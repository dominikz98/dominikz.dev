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
using dominikz.Infrastructure.Clients.Api;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Pages.Movies;

public partial class Movies : ComponentBase
{
    [Inject] protected MovieEndpoints? MovieEndpoints { get; set; }
    [Inject] protected NavigationManager? NavManager { get; set; }
    [Inject] protected BrowserService? Browser { get; set; }
    [Inject] protected ToastService? Toast { get; set; }
    [Inject] protected ICredentialStorage? Credentials { get; set; }

    private List<MoviePreviewVm> _previews = new();
    private List<MovieVm> _movies = new();
    private bool _hasCreatePermission;

    private bool _isTableView;
    private TextBox? _searchbox;
    private ChipSelect<MovieGenresFlags>? _movieGenreSelect;

    protected override async Task OnInitializedAsync()
    {
        _hasCreatePermission = await Credentials!.HasRight(PermissionFlags.CreateOrUpdate | PermissionFlags.Movies);
        _previews = await MovieEndpoints!.GetPreview();

        var filter = CreateFilter();
        _searchbox?.SetValue(filter.Text);
        _movieGenreSelect?.Select(filter.Genres);

        var init = NavManager!.TrackQuery(SearchByCategory);
        if (init)
            await SearchByCategory();
    }

    private async Task OnCopyLinkClicked()
    {
        await Browser!.CopyToClipboard(NavManager!.Uri);
        Toast!.Show("Link stored in clipboard", ToastLevel.Success);
    }

    private async Task OnCreateCURLClicked()
    {
        var filter = CreateFilter();
        var curl = MovieEndpoints!.CurlSearch(filter);

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

    private async Task SearchByCategory()
    {
        var filter = CreateFilter();
        _movies = await MovieEndpoints!.Search(filter);
        StateHasChanged();
    }

    private void NavigateToMovie(Guid movieId)
        => NavManager!.NavigateTo($"/movies/{movieId}");
}