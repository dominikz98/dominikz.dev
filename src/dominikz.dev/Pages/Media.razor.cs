using dominikz.kernel.Endpoints;
using dominikz.kernel.ViewModels;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Pages;

public partial class Media
{
    [Inject]
    protected MediasEndpoints? Endpoints { get; set; }

    private List<MediaVM> _preview = new();
    private List<MediaVM> _media = new();
    private Dictionary<MediaCategoryEnum, List<MediaGenre>> _genresByCategory = new();

    private string? _search;
    private MediaCategoryEnum _category;
    private MediaGenre _genre;

    protected override async Task OnInitializedAsync()
    {
        await LoadPreview();
        await SearchMedia();
        FillGenresDictionary();
    }

    private async Task LoadPreview()
    {
        var filter = MediaFilter.Preview;
        _preview = await Endpoints!.Search(filter);
    }

    private async Task OnSearchChanged(string? value)
    {
        _search = value;
        await SearchMedia();
    }

    private async Task OnCategoryChanged(List<MediaCategoryEnum> value)
    {
        _category = value.FirstOrDefault();
        await SearchMedia();
    }

    private async Task OnGenreChanged(List<MediaGenre> value)
    {
        _genre = value.FirstOrDefault();
        await SearchMedia();
    }

    private async Task SearchMedia()
    {
        var filter = new MediaFilter
        {
            Text = _search,
            Category = _category,
            Genre = _genre
        };
        _media = await Endpoints!.Search(filter);
    }

    private void FillGenresDictionary()
        => _genresByCategory = _media
            .GroupBy(x => x.Category)
            .Select(x => new
            {
                x.Key,
                Genres = x.SelectMany(y => y.Genres)
                    .Distinct()
                    .ToList()
            })
            .ToDictionary(x => x.Key, x => x.Genres);
}
