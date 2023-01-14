using dominikz.Client.Extensions;
using dominikz.Client.Wrapper;
using dominikz.Domain.Enums;
using dominikz.Domain.Enums.Media;
using dominikz.Domain.Structs;
using dominikz.Domain.ViewModels;
using dominikz.Infrastructure.Clients.Api;
using dominikz.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace dominikz.Client.Pages.Media;

public partial class EditMovie
{
    [Parameter] public Guid? MovieId { get; set; }
    [Inject] internal MovieEndpoints? MovieEndpoints { get; set; }
    [Inject] internal DownloadEndpoints? DownloadEndpoints { get; set; }
    [Inject] internal NavigationManager? NavManager { get; set; }

    private EditContext? _editContext;
    private EditMovieWrapper _vm = new();
    private List<FileStruct> _posterFiles = new();
    private List<string> _genreRecommendations = new();
    private bool _isEnabled;

    protected override async Task OnInitializedAsync()
    {
        var movieLoaded = await LoadMovieIfRequired();
        if (movieLoaded == false)
        {
            _vm.Id = Guid.NewGuid();
            _vm.PublishDate = DateTime.UtcNow.AddDays(1);
            _vm.Rating = 50;
        }

        _editContext = new EditContext(_vm);
        _isEnabled = true;
    }

    private async Task<bool> LoadMovieIfRequired()
    {
        if (MovieId == null)
            return false;

        var movie = await MovieEndpoints!.GetDraftById(MovieId.Value);
        if (movie == null)
            return false;

        _vm = new EditMovieWrapper()
        {
            Id = _vm.Id,
            Plot = _vm.Plot,
            Runtime = _vm.Runtime,
            Title = _vm.Title,
            YouTubeId = _vm.YouTubeId,
            JustWatchId = _vm.JustWatchId,
            ImdbId = _vm.ImdbId,
            PublishDate = _vm.PublishDate,
            Year = _vm.Year,
            Rating = _vm.Rating,
            Genres = _vm.Genres,
            Comment = _vm.Comment,
            Directors = _vm.Directors,
            Writers = _vm.Writers,
            Stars = _vm.Stars
        };

        var file = await DownloadEndpoints!.Image(movie.Id, true, ImageSizeEnum.Original);
        if (file == null)
            return true;

        _vm.Image.Add(file.Value);
        return true;
    }

    private void OnJustWatchIdChanged(string? jwIdRaw)
    {
        if (int.TryParse(jwIdRaw, out var jwId) == false)
            return;

        _vm.JustWatchId = jwId;
    }

    private async Task OnImdbIdChanged(string? imdbId)
    {
        _vm.ImdbId = imdbId ?? string.Empty;
        if (string.IsNullOrWhiteSpace(imdbId))
            return;

        if (MovieId != null)
            return;

        var data = await MovieEndpoints!.GetTemplateByImdbId(imdbId);
        if (data == null)
            return;

        if (_vm.JustWatchId == default)
            _vm.JustWatchId = data.JustWatchId;

        if (_vm.YouTubeId == string.Empty)
            _vm.YouTubeId = data.YouTubeId;

        if (_vm.Title == string.Empty)
            _vm.Title = data.Title;

        if (_vm.Plot == string.Empty)
            _vm.Plot = data.Plot;

        if (_vm.Runtime == default)
            _vm.Runtime = data.Runtime;

        if (_vm.Year == default)
            _vm.Year = data.Year;

        if (_vm.Rating is 0 or 50)
            _vm.Rating = data.Rating;

        if (_vm.Directors.Count == 0)
            _vm.DirectorsWrappers = data.Directors.Select(x => x.Wrap()).ToList();

        if (_vm.Writers.Count == 0)
            _vm.WritersWrappers = data.Writers.Select(x => x.Wrap()).ToList();

        if (_vm.Stars.Count == 0)
            _vm.StarsWrappers = data.Stars.Select(x => x.Wrap()).ToList();

        if (_vm.Genres.Count == 0)
        {
            _genreRecommendations = data.GenreRecommendations.OrderBy(x => x).ToList();
            _vm.Genres = Enum.GetValues<MovieGenresFlags>()[1..]
                .Where(x => _genreRecommendations.ContainsCleaned(x.ToString()))
                .ToList();

            _genreRecommendations = _genreRecommendations.Where(x => _vm.Genres.ContainsCleaned(x) == false).ToList();
        }

        if (_vm.Image.Count == 0 && _posterFiles.Count == 0)
        {
            foreach (var posterUrl in data.PosterUrls)
            {
                var posterFile = await DownloadEndpoints!.RawImage(posterUrl);
                if (posterFile == null)
                    return;

                _posterFiles.Add(posterFile.Value);
            }

            if (_posterFiles.Count > 0)
            {
                var toSelect = _posterFiles.First();
                _vm.Image.Add(toSelect);
                _posterFiles.Remove(toSelect);
            }
        }
    }

    private async Task OnSaveClicked()
    {
        PrepareVm();

        if (_editContext == null || _editContext.Validate() == false)
            return;
        
        var files = _vm.StarsWrappers.SelectMany(x => x.Image)
            .Union(_vm.DirectorsWrappers.SelectMany(x => x.Image))
            .Union(_vm.WritersWrappers.SelectMany(x => x.Image))
            .ToList();
        
        var movie = MovieId == null
            ? await MovieEndpoints!.Add(_vm, files)
            : await MovieEndpoints!.Update(_vm, files);
        
        if (movie == null)
            return;
        
        NavManager?.NavigateTo($"/media/movie/{movie.Id}");
    }

    private void PrepareVm()
    {
        // Set File Ids
        for (var i = 0; i < _vm.Image.Count; i++)
            _vm.Image[i] = _vm.Image[i].CoptyTo(_vm.Id.ToString());

        foreach (var director in _vm.DirectorsWrappers)
        {
            for (var a = 0; a < director.Image.Count; a++)
            {
                director.Image[a] = director.Image[a].CoptyTo(director.Id.ToString());
            }
        }
        
        foreach (var writer in _vm.WritersWrappers)
            for (var a = 0; a < writer.Image.Count; a++)
                writer.Image[a] = writer.Image[a].CoptyTo(writer.Id.ToString());

        foreach (var star in _vm.StarsWrappers)
        {
            for (var a = 0; a < star.Image.Count; a++)
            {
                star.Image[a] = star.Image[a].CoptyTo(star.Id.ToString());
            }
        }

        // Cast persons back
        _vm.Directors = _vm.DirectorsWrappers.Select(x => (EditPersonVm)x).ToList();
        _vm.Writers = _vm.WritersWrappers.Select(x => (EditPersonVm)x).ToList();
        _vm.Stars = _vm.StarsWrappers.Select(x => (EditPersonVm)x).ToList();
    }
}