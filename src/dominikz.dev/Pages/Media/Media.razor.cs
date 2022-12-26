using dominikz.dev.Components;
using dominikz.dev.Components.Chips;
using dominikz.dev.Components.TabControl;
using dominikz.dev.Components.Toast;
using dominikz.dev.Endpoints;
using dominikz.dev.Models;
using dominikz.dev.Utils;
using dominikz.shared.Contracts;
using dominikz.shared.Filter;
using dominikz.shared.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace dominikz.dev.Pages.Media;

public partial class Media
{
    [Inject] protected MediaEndpoints? MediaEndpoints { get; set; }
    [Inject] protected MovieEndpoints? MovieEndpoints { get; set; }
    [Inject] protected GameEndpoints? GameEndpoints { get; set; }
    [Inject] protected BookEndpoints? BookEndpoints { get; set; }
    [Inject] protected NavigationManager? NavManager { get; set; }
    [Inject] protected BrowserService? Browser { get; set; }
    [Inject] protected ToastService? Toast { get; set; }

    private List<MediaPreviewVM> _previews = new();
    private List<MovieVM> _movies = new();
    private List<GameVM> _games = new();
    private List<BookVM> _books = new();

    private const string QueryView = "view";
    private const string QuerySearch = "search";
    private const string QueryCategory = "category";
    private const string QueryMovieGenre = "m_genre";
    private const string QueryBookGenre = "b_genre";
    private const string QueryBookLanguage = "b_language";
    private const string QueryGameGenre = "g_genre";
    private const string QueryGamePlatform = "g_platform";

    private int _view = (int)CollectionView.Grid;

    private Searchbox? _searchbox;
    private TabControl? _categoryTabCtrl;
    private ChipSelect<MovieGenresFlags>? _movieGenreSelect;
    private ChipSelect<BookGenresFlags>? _bookGenreSelect;
    private ChipSelect<BookLanguageEnum>? _bookLanguageSelect;
    private ChipSelect<GameGenresFlags>? _gameGenreSelect;
    private ChipSelect<GamePlatformEnum>? _gamePlatformSelect;

    protected override async Task OnInitializedAsync()
    {
        _previews = await MediaEndpoints!.GetPreview();
        NavManager!.LocationChanged += async (_, _) => await SearchByCategory();
        await SearchByCategory();

        var search = NavManager.GetQueryParamByKey(QuerySearch);
        _searchbox?.SetValue(search);

        var category = NavManager.GetQueryParamByKey<MediaCategoryEnum>(QueryCategory) ?? MediaCategoryEnum.Movie;
        _categoryTabCtrl?.ShowPage((int)category);

        var movieGenre = NavManager.GetQueryParamByKey<MovieGenresFlags>(QueryMovieGenre);
        _movieGenreSelect?.Select(movieGenre);

        var bookGenre = NavManager.GetQueryParamByKey<BookGenresFlags>(QueryBookGenre);
        _bookGenreSelect?.Select(bookGenre);

        var bookLanguage = NavManager.GetQueryParamByKey<BookLanguageEnum>(QueryBookLanguage);
        _bookLanguageSelect?.Select(bookLanguage);

        var gameGenre = NavManager.GetQueryParamByKey<GameGenresFlags>(QueryGameGenre);
        _gameGenreSelect?.Select(gameGenre);

        var gamePlatform = NavManager.GetQueryParamByKey<GamePlatformEnum>(QueryGamePlatform);
        _gamePlatformSelect?.Select(gamePlatform);
    }

    private void OnPageChanged(int pageId)
    {
        if (Enum.TryParse<MediaCategoryEnum>(pageId.ToString(), out var category) == false)
            return;

        // remove filter and change category
        var parameter = new Dictionary<string, string>();
        if (category != MediaCategoryEnum.Movie)
            parameter.Add(QueryCategory, category.ToString().ToLower());

        var search = NavManager!.GetQueryParamByKey(QuerySearch);
        if (string.IsNullOrWhiteSpace(search) == false)
            parameter.Add(QuerySearch, search);

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
        var category = NavManager!.GetQueryParamByKey<MediaCategoryEnum>(QueryCategory) ?? MediaCategoryEnum.Movie;
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
                    Text = NavManager!.GetQueryParamByKey(QuerySearch),
                    Genres = NavManager!.GetQueryParamByKey<MovieGenresFlags>(QueryMovieGenre)
                };
            case MediaCategoryEnum.Book:
                return new BooksFilter()
                {
                    Text = NavManager!.GetQueryParamByKey(QuerySearch),
                    Genres = NavManager!.GetQueryParamByKey<BookGenresFlags>(QueryBookGenre),
                    Language = NavManager!.GetQueryParamByKey<BookLanguageEnum>(QueryBookLanguage)
                };
            case MediaCategoryEnum.Game:
                return new GamesFilter()
                {
                    Text = NavManager!.GetQueryParamByKey(QuerySearch),
                    Genres = NavManager!.GetQueryParamByKey<GameGenresFlags>(QueryGameGenre),
                    Platform = NavManager!.GetQueryParamByKey<GamePlatformEnum>(QueryGamePlatform)
                };
        }

        throw new NotImplementedException("Filter not implemented!");
    }

    private async Task SearchByCategory()
    {
        var category = NavManager!.GetQueryParamByKey<MediaCategoryEnum>(QueryCategory) ?? MediaCategoryEnum.Movie;
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