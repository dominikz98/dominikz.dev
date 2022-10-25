using dominikz.dev.Components;
using dominikz.dev.Definitions;
using dominikz.dev.Endpoints;
using dominikz.kernel.Contracts;
using dominikz.kernel.Filter;
using dominikz.kernel.ViewModels;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Pages.Media;

public partial class Media
{
    [Inject]
    protected MediaEndpoints? MediaEndpoints { get; set; }

    [Inject]
    protected MovieEndpoints? MovieEndpoints { get; set; }

    [Inject]
    protected GameEndpoints? GameEndpoints { get; set; }

    [Inject]
    protected BookEndpoints? BookEndpoints { get; set; }

    [Inject]
    protected NavigationManager? Navigation { get; set; }

    // data
    private List<MediaPreviewVM> _previews = new();
    private List<MovieVM> _movies = new();
    private List<GameVM> _games = new();
    private List<BookVM> _books = new();

    // searchbar
    private string? _search;
    private OrderInfo? _order;
    private List<string> _orderKeys = new();
    private CollectionView _view;

    // filter
    private MediaCategoryEnum _category;
    private GameGenresFlags _gameGenres;
    private GamePlatformEnum _gamePlatform;
    private BookGenresFlags _bookGenres;
    private BookLanguageEnum _bookLanguage;

    protected override async Task OnInitializedAsync()
       => _previews = await MediaEndpoints!.GetPreview();

    private async Task OnSearchChanged(string value)
    {
        _search = value;
        await LoadByCategory();
    }

    private async Task OnCategoriesChanged(List<MediaCategoryEnum> categories)
    {
        _category = categories.FirstOrDefault();
        await LoadByCategory();
    }

    private async Task OnMovieGenreChanged(List<MovieGenresFlags> genres)
    {
        var genre = (MovieGenresFlags)genres.Sum(x => (int)x);
        await LoadMovies(genre);
        OrderByCategory();
    }

    private async Task OnGameGenreChanged(List<GameGenresFlags> genres)
    {
        var genre = (GameGenresFlags)genres.Sum(x => (int)x);
        _gameGenres = genre;
        await LoadGames();
        OrderByCategory();
    }

    private async Task OnGamePlatformChanged(List<GamePlatformEnum> platforms)
    {
        _gamePlatform = platforms.FirstOrDefault();
        await LoadGames();
        OrderByCategory();
    }

    private async Task OnBookGenreChanged(List<BookGenresFlags> genres)
    {
        var genre = (BookGenresFlags)genres.Sum(x => (int)x);
        _bookGenres = genre;
        await LoadBooks();
        OrderByCategory();
    }

    private async Task OnBookLanguageChanged(List<BookLanguageEnum> language)
    {
        _bookLanguage = language.FirstOrDefault();
        await LoadBooks();
        OrderByCategory();
    }

    private void OnOrderChanged(OrderInfo order)
    {
        _order = order;
        OrderByCategory();
    }

    private async Task LoadByCategory()
    {
        switch (_category)
        {
            case MediaCategoryEnum.Movie:
                _orderKeys = MovieTableDefinition.OrderKeys;
                await LoadMovies(MovieGenresFlags.ALL);
                break;
            case MediaCategoryEnum.Book:
                _orderKeys = BookTableDefinition.OrderKeys;
                await LoadBooks();
                break;
            case MediaCategoryEnum.Game:
                _orderKeys = GameTableDefinition.OrderKeys;
                await LoadGames();
                break;
            default:
                break;
        }

        OrderByCategory();
        StateHasChanged();
    }

    private async Task LoadMovies(MovieGenresFlags genres)
    {
        var filter = new MoviesFilter()
        {
            Text = _search,
            Genres = genres
        };
        _movies = await MovieEndpoints!.Search(filter);
    }

    private async Task LoadGames()
    {
        var filter = new GamesFilter()
        {
            Text = _search,
            Genres = _gameGenres,
            Platform = _gamePlatform
        };
        _games = await GameEndpoints!.Search(filter);
    }

    private async Task LoadBooks()
    {
        var filter = new BooksFilter()
        {
            Text = _search,
            Genres = _bookGenres,
            Language = _bookLanguage
        };
        _books = await BookEndpoints!.Search(filter);
    }

    private void OrderByCategory()
    {
        if (_order is null)
            return;

        switch (_category)
        {
            case MediaCategoryEnum.Movie:
                _movies = _movies.OrderByKey(_order).ToList();
                break;
            case MediaCategoryEnum.Book:
                _books = _books.OrderByKey(_order).ToList();
                break;
            case MediaCategoryEnum.Game:
                _games = _games.OrderByKey(_order).ToList();
                break;
            default:
                break;
        }
    }

    private void NavigateToMovie(Guid movieId)
        => Navigation!.NavigateTo($"/media/movie/{movieId}");
}
