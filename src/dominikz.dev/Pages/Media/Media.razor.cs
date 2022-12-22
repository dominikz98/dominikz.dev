using dominikz.dev.Components;
using dominikz.dev.Components.Chips;
using dominikz.dev.Endpoints;
using dominikz.dev.Models;
using dominikz.shared.Contracts;
using dominikz.shared.Filter;
using dominikz.shared.ViewModels;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Pages.Media;

public partial class Media
{
    [Inject] protected MediaEndpoints? MediaEndpoints { get; set; }

    [Inject] protected MovieEndpoints? MovieEndpoints { get; set; }

    [Inject] protected GameEndpoints? GameEndpoints { get; set; }

    [Inject] protected BookEndpoints? BookEndpoints { get; set; }

    [Inject] protected NavigationManager? NavManager { get; set; }

    // data
    private List<MediaPreviewVM> _previews = new();
    private List<MovieVM> _movies = new();
    private List<GameVM> _games = new();
    private List<BookVM> _books = new();

    private readonly CollectionView _view = CollectionView.Grid;
    private Searchbox? _searchbox;
    private ChipSelect<MediaCategoryEnum>? _categorySelect;
    private ChipSelect<MovieGenresFlags>? _movieGenresSelect;
    private ChipSelect<BookGenresFlags>? _bookGenresSelect;
    private ChipSelect<BookLanguageEnum>? _bookLanguageSelect;
    private ChipSelect<GameGenresFlags>? _gameGenresSelect;
    private ChipSelect<GamePlatformEnum>? _gamePlatformSelect;

    private MediaCategoryEnum Category
        => _categorySelect?.Selected ?? MediaCategoryEnum.Movie;

    protected override async Task OnInitializedAsync()
    {
        if (_previews.Count == 0)
            _previews = await MediaEndpoints!.GetPreview();

        NavManager!.LocationChanged += async (_, _) => await SearchByCategory();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender == false)
            return;

        await SearchByCategory();
    }

    private async Task SearchByCategory()
    {
        switch (Category)
        {
            case MediaCategoryEnum.Movie:
                var movieFilter = new MoviesFilter()
                {
                    Text = _searchbox?.Value,
                    Genres = _movieGenresSelect?.Selected
                };
                _movies = await MovieEndpoints!.Search(movieFilter);
                break;
            case MediaCategoryEnum.Book:
                var bookFilter = new BooksFilter()
                {
                    Text = _searchbox?.Value,
                    Genres = _bookGenresSelect?.Selected,
                    Language = _bookLanguageSelect?.Selected
                };
                _books = await BookEndpoints!.Search(bookFilter);
                break;
            case MediaCategoryEnum.Game:
                var gameFilter = new GamesFilter()
                {
                    Text = _searchbox?.Value,
                    Genres = _gameGenresSelect?.Selected,
                    Platform = _gamePlatformSelect?.Selected
                };
                _games = await GameEndpoints!.Search(gameFilter);
                break;
        }

        StateHasChanged();
    }

    private void NavigateToMovie(Guid movieId)
        => NavManager!.NavigateTo($"/media/movie/{movieId}");
}