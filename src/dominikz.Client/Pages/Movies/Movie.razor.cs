using dominikz.Client.Api;
using dominikz.Domain.Enums;
using dominikz.Domain.ViewModels.Movies;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Pages.Movies;

public partial class Movie
{
    [Parameter] public Guid MovieId { get; set; }
    [Inject] protected MovieEndpoints? MovieEndpoints { get; set; }
    [Inject] protected DownloadEndpoints? DownloadEndpoints { get; set; }
    [Inject] protected ICredentialStorage? Credentials { get; set; }
    [Inject] protected NavigationManager? NavManager { get; set; }

    private MovieDetailVm? _movie;
    private bool _hasCreatePermission;
    private bool _hasStreamPermission;
    private string _movieStreamUrl = string.Empty;
    private string _trailerStreamUrl = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        _hasCreatePermission = await Credentials!.HasRight(PermissionFlags.CreateOrUpdate | PermissionFlags.Movies);
        _hasStreamPermission = await Credentials!.HasRight(PermissionFlags.Movies);

        _movie = await MovieEndpoints!.GetById(MovieId);
        if (_movie == null)
            return;

        if (_movie.IsTrailerStreamAvailable)
        {
            var stream = await DownloadEndpoints!.CreateTrailerStreamingToken(MovieId);
            if (string.IsNullOrWhiteSpace(stream?.Token) == false)
                _trailerStreamUrl = DownloadEndpoints!.CreateTrailerStreamingUrl(MovieId, stream.Token);
        }

        if (_hasStreamPermission && _movie.IsStreamAvailable)
        {
            var stream = await DownloadEndpoints!.CreateMovieStreamingToken(MovieId);
            if (string.IsNullOrWhiteSpace(stream?.Token) == false)
                _movieStreamUrl = DownloadEndpoints!.CreateMovieStreamingUrl(MovieId, stream.Token);
        }
    }
}