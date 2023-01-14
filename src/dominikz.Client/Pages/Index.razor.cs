using dominikz.Client.Utils;
using dominikz.Domain.Constants;
using dominikz.Domain.Enums;
using dominikz.Domain.Enums.Media;
using dominikz.Domain.Structs;
using dominikz.Infrastructure.Clients.Api;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Pages;

public partial class Index
{
    [Inject] public DownloadEndpoints? DownloadEndpoints { get; set; }
    [Inject] public NavigationManager? NavigationManager { get; set; }
    
    private static Random _rnd = new();
    private const int RandomGenresCount = 3;

    private FileStruct? _authorImage;
    private FileStruct? _noobitImage;
    private FileStruct? _medlanImage;

    protected override async Task OnInitializedAsync()
    {
        _authorImage = await DownloadEndpoints!.Image(Persons.DominikZettlId, false, ImageSizeEnum.Original);
        _noobitImage = await DownloadEndpoints!.Image(Persons.TobiasHaimerlId, false, ImageSizeEnum.Original);
        _medlanImage = await DownloadEndpoints!.Image(Persons.MarkusLieblId, false, ImageSizeEnum.Original);
    }

    private string GetRandomGenres()
    {
        var allGenres = Enum.GetValues<MovieGenresFlags>().ToList();
        var ix = _rnd.Next(0, allGenres.Count - RandomGenresCount);
        var genres=  allGenres.GetRange(ix, RandomGenresCount);
        return string.Join(", ", EnumFormatter.ToString(genres));
    }
}