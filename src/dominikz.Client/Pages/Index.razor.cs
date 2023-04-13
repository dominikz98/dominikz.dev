using dominikz.Client.Utils;
using dominikz.Domain.Enums.Movies;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Pages;

public partial class Index
{
    [Inject] public NavigationManager? NavigationManager { get; set; }
    
    private static readonly Random _rnd = new();
    private const int RandomGenresCount = 3;

    private string GetRandomGenres()
    {
        var allGenres = Enum.GetValues<MovieGenresFlags>().ToList();
        var ix = _rnd.Next(0, allGenres.Count - RandomGenresCount);
        var genres=  allGenres.GetRange(ix, RandomGenresCount);
        return string.Join(", ", EnumFormatter.ToString(genres));
    }
}