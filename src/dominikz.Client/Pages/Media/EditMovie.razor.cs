using dominikz.Client.Wrapper;
using dominikz.Domain.Enums;
using dominikz.Domain.Enums.Media;
using dominikz.Domain.Structs;
using dominikz.Domain.ViewModels.Media;
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
    private EditWithImageWrapper<EditMovieVm> _data = new();
    private List<FileStruct> _posterFiles = new();
    private List<string> _genreRecommendations = new();
    private bool _isEnabled;

    protected override async Task OnInitializedAsync()
    {
        var movieLoaded = await LoadMovieIfRequired();
        if (movieLoaded == false)
        {
            _data.ViewModel.Id = Guid.NewGuid();
            _data.ViewModel.PublishDate = DateTime.UtcNow.AddDays(1);
            _data.ViewModel.Rating = 50;
        }

        _editContext = new EditContext(_data);
        _isEnabled = true;
    }

    private async Task<bool> LoadMovieIfRequired()
    {
        if (MovieId == null)
            return false;

        var movie = await MovieEndpoints!.GetDraftById(MovieId.Value);
        if (movie == null)
            return false;

        _data.ViewModel = movie;
        var file = await DownloadEndpoints!.Image(movie.Id, true, ImageSizeEnum.Original);
        if (file == null)
            return true;

        _data.Image.Add(file.Value);
        return true;
    }

    private async Task OnImdbIdChanged(string? imdbId)
    {
        _data.ViewModel.ImdbId = imdbId ?? string.Empty;
        if (string.IsNullOrWhiteSpace(imdbId))
            return;

        if (MovieId != null)
            return;

        var data = await MovieEndpoints!.GetTemplateByImdbId(imdbId);
        if (data == null)
            return;

        if (_data.ViewModel.Title == string.Empty)
            _data.ViewModel.Title = data.Title;

        if (_data.ViewModel.Plot == string.Empty)
            _data.ViewModel.Plot = data.Plot;

        if (_data.ViewModel.Runtime == default)
            _data.ViewModel.Runtime = data.Runtime;

        if (_data.ViewModel.Year == default)
            _data.ViewModel.Year = data.Year;

        if (_data.ViewModel.Rating is 0 or 50)
            _data.ViewModel.Rating = data.Rating;

        if (_data.ViewModel.Genres.Count == 0)
        {
            _genreRecommendations = data.GenreRecommendations.OrderBy(x => x).ToList();
            _data.ViewModel.Genres = Enum.GetValues<MovieGenresFlags>()[1..]
                .Where(x => _genreRecommendations.ContainsCleaned(x.ToString()))
                .ToList();

            _genreRecommendations = _genreRecommendations.Where(x => _data.ViewModel.Genres.ContainsCleaned(x) == false).ToList();
        }

        if (_data.Image.Count == 0 && _posterFiles.Count == 0)
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
                _data.Image.Add(toSelect);
                _posterFiles.Remove(toSelect);
            }
        }
    }

    private async Task OnSaveClicked()
    {
        // Set File Ids
        for (var i = 0; i < _data.Image.Count; i++)
            _data.Image[i] = _data.Image[i].CopyTo(_data.ViewModel.Id.ToString());

        if (_editContext == null || _editContext.Validate() == false)
            return;

        var movie = MovieId == null
            ? await MovieEndpoints!.Add(_data.ViewModel, _data.Image)
            : await MovieEndpoints!.Update(_data.ViewModel, _data.Image);

        if (movie == null)
            return;

        NavManager?.NavigateTo($"/media/movie/{movie.Id}");
    }
}