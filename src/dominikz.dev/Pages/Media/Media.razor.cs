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
    protected NavigationManager? Navigation { get; set; }

    // data
    private List<MediaPreviewVM> _previews = new();
    private List<MovieVM> _movies = new();

    // searchbar
    private string? _search;
    private OrderInfo? _order;
    private List<string> _orderKeys = new();
    private CollectionView _view;

    // filter
    private MediaCategoryEnum _category;

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

    private async Task OnMovieGenreChanged(List<MovieGenreFlags> genres)
    {
        var genre = (MovieGenreFlags)genres.Sum(x => (int)x);
        await LoadMovies(genre);
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
                await LoadMovies(MovieGenreFlags.ALL);
                break;
            case MediaCategoryEnum.Series:
                break;
            case MediaCategoryEnum.Book:
                break;
            case MediaCategoryEnum.Game:
                break;
            default:
                break;
        }

        OrderByCategory();
        StateHasChanged();
    }

    private async Task LoadMovies(MovieGenreFlags genres)
    {
        var filter = new MoviesFilter()
        {
            Text = _search,
            Genres = genres
        };
        _movies = await MovieEndpoints!.Search(filter);
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
            case MediaCategoryEnum.Series:
                break;
            case MediaCategoryEnum.Book:
                break;
            case MediaCategoryEnum.Game:
                break;
            default:
                break;
        }
    }

    private void NavigateToMovie(Guid movieId)
        => Navigation!.NavigateTo($"/media/movie/{movieId}");
}
