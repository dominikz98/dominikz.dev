using dominikz.Client.Components;
using dominikz.Client.Components.Chips;
using dominikz.Client.Components.TabControl;
using dominikz.Client.Components.Toast;
using dominikz.Client.Extensions;
using dominikz.Client.Tables;
using dominikz.Client.Utils;
using dominikz.Domain.Contracts;
using dominikz.Domain.Enums;
using dominikz.Domain.Filter;
using dominikz.Domain.ViewModels.Media;
using dominikz.Infrastructure.Clients.Api;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace dominikz.Client.Pages.Media;

public partial class Media
{
    [Inject] protected MediaEndpoints? MediaEndpoints { get; set; }
    [Inject] protected MovieEndpoints? MovieEndpoints { get; set; }
    [Inject] protected GameEndpoints? GameEndpoints { get; set; }
    [Inject] protected BookEndpoints? BookEndpoints { get; set; }
    [Inject] protected NavigationManager? NavManager { get; set; }
    [Inject] protected BrowserService? Browser { get; set; }
    [Inject] protected ToastService? Toast { get; set; }
    [Inject] protected ICredentialStorage? Credentials { get; set; }

    private List<MediaPreviewVm> _previews = new();
    private List<MovieVm> _movies = new();
    private List<GameVm> _games = new();
    private List<BookVm> _books = new();
    private bool _hasCreatePermission;
    
    private bool _isTableView;
    private TextBox? _searchbox;
    private TabControl? _categoryTabCtrl;
    private ChipSelect<MovieGenresFlags>? _movieGenreSelect;
    private ChipSelect<BookGenresFlags>? _bookGenreSelect;
    private ChipSelect<BookLanguageEnum>? _bookLanguageSelect;
    private ChipSelect<GameGenresFlags>? _gameGenreSelect;
    private ChipSelect<GamePlatformEnum>? _gamePlatformSelect;

    protected override async Task OnInitializedAsync()
    {
        _hasCreatePermission = await Credentials!.HasRight(PermissionFlags.CreateOrUpdate | PermissionFlags.Media);
        
        _previews = await MediaEndpoints!.GetPreview();
        NavManager!.LocationChanged += async (_, _) => await SearchByCategory();
        await SearchByCategory();

        var category = NavManager.GetQueryParamByKey<MediaCategoryEnum>(QueryNames.Media.Category) ?? MediaCategoryEnum.Movie;
        _categoryTabCtrl?.ShowPage((int)category);

        var search = NavManager.GetQueryParamByKey(QueryNames.Media.Search);
        _searchbox?.SetValue(search);

        var filter = CreateFilterByCategory(category);
        if (filter is MoviesFilter mFilter)
            _movieGenreSelect?.Select(mFilter.Genres);

        else if (filter is BooksFilter bFilter)
        {
            _bookGenreSelect?.Select(bFilter.Genres);
            _bookLanguageSelect?.Select(bFilter.Language);    
        }
        else if (filter is GamesFilter gFilter)
        {
            _gameGenreSelect?.Select(gFilter.Genres);
            _gamePlatformSelect?.Select(gFilter.Platform);    
        }
    }

    private void OnPageChanged(int pageId)
    {
        if (Enum.TryParse<MediaCategoryEnum>(pageId.ToString(), out var category) == false)
            return;

        // remove filter and change category
        var parameter = new Dictionary<string, string>();
        if (category != MediaCategoryEnum.Movie)
            parameter.Add(QueryNames.Media.Category, category.ToString().ToLower());

        var search = NavManager!.GetQueryParamByKey(QueryNames.Media.Search);
        if (string.IsNullOrWhiteSpace(search) == false)
            parameter.Add(QueryNames.Media.Search, search);

        // navigate to updated url
        var url = NavManager!.ToAbsoluteUri(NavManager.Uri).GetLeftPart(UriPartial.Path);
        url = QueryHelpers.AddQueryString(url, parameter);
        NavManager.NavigateTo(url);
    }

    private async Task OnCopyLinkClicked()
    {
        await Browser!.CopyToClipboard(NavManager!.Uri);
        Toast!.Show("Link stored in clipboard", ToastLevel.Success);
    }

    private async Task OnCreateCURLClicked()
    {
        var category = NavManager!.GetQueryParamByKey<MediaCategoryEnum>(QueryNames.Media.Category) ?? MediaCategoryEnum.Movie;
        var filter = CreateFilterByCategory(category);
        var curl = category switch
        {
            MediaCategoryEnum.Movie => MovieEndpoints!.CurlSearch((MoviesFilter)filter),
            MediaCategoryEnum.Book => BookEndpoints!.CurlSearch((BooksFilter)filter),
            MediaCategoryEnum.Game => GameEndpoints!.CurlSearch((GamesFilter)filter),
            _ => throw new ArgumentOutOfRangeException()
        };

        if (string.IsNullOrWhiteSpace(curl))
        {
            Toast!.Show("Error creating CURL", ToastLevel.Error);
            return;
        }

        await Browser!.CopyToClipboard(curl);
        Toast!.Show("CURL stored in clipboard", ToastLevel.Success);
    }

    private IFilter CreateFilterByCategory(MediaCategoryEnum category)
    {
        switch (category)
        {
            case MediaCategoryEnum.Movie:
                return new MoviesFilter()
                {
                    Text = NavManager!.GetQueryParamByKey(QueryNames.Media.Search),
                    Genres = NavManager!.GetQueryParamByKey<MovieGenresFlags>(QueryNames.Media.Movie.Genre)
                };
            case MediaCategoryEnum.Book:
                return new BooksFilter()
                {
                    Text = NavManager!.GetQueryParamByKey(QueryNames.Media.Search),
                    Genres = NavManager!.GetQueryParamByKey<BookGenresFlags>(QueryNames.Media.Book.Genre),
                    Language = NavManager!.GetQueryParamByKey<BookLanguageEnum>(QueryNames.Media.Book.Language)
                };
            case MediaCategoryEnum.Game:
                return new GamesFilter()
                {
                    Text = NavManager!.GetQueryParamByKey(QueryNames.Media.Search),
                    Genres = NavManager!.GetQueryParamByKey<GameGenresFlags>(QueryNames.Media.Game.Genre),
                    Platform = NavManager!.GetQueryParamByKey<GamePlatformEnum>(QueryNames.Media.Game.Platform)
                };
        }

        throw new NotImplementedException("Filter not implemented!");
    }

    private async Task SearchByCategory()
    {
        var category = NavManager!.GetQueryParamByKey<MediaCategoryEnum>(QueryNames.Media.Category) ?? MediaCategoryEnum.Movie;
        var filter = CreateFilterByCategory(category);
        switch (category)
        {
            case MediaCategoryEnum.Movie:
                _movies = await MovieEndpoints!.Search((MoviesFilter)filter);
                break;
            case MediaCategoryEnum.Book:
                _books = await BookEndpoints!.Search((BooksFilter)filter);
                break;
            case MediaCategoryEnum.Game:
                _games = await GameEndpoints!.Search((GamesFilter)filter);
                break;
        }

        StateHasChanged();
    }

    private void NavigateToMovie(Guid movieId)
        => NavManager!.NavigateTo($"/media/movie/{movieId}");
}