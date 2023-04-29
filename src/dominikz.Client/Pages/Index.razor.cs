using dominikz.Client.Utils;
using dominikz.Domain.Enums.Movies;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Pages;

public partial class Index
{
    [Inject] public NavigationManager? NavigationManager { get; set; }
    [Inject] public BrowserService? Browser { get; set; }

    private static readonly Random Rnd = new();
    private const int RandomGenresCount = 3;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var streamingMode = await Browser!.IsStreamingModeEnabled();
        if (streamingMode)
            NavigationManager!.NavigateTo("/movies");
    }

    private string GetRandomGenres()
    {
        var allGenres = Enum.GetValues<MovieGenresFlags>().ToList();
        var ix = Rnd.Next(0, allGenres.Count - RandomGenresCount);
        var genres = allGenres.GetRange(ix, RandomGenresCount);
        return string.Join(", ", EnumFormatter.ToString(genres));
    }
}