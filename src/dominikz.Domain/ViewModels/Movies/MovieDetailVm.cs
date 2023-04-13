namespace dominikz.Domain.ViewModels.Movies;

public class MovieDetailVm : MovieVm
{
    public string? Comment { get; init; }
    public string Plot { get; init; } = string.Empty;
    public TimeSpan Runtime { get; init; }
    public bool IsStreamAvailable { get; set; }
    public bool IsTrailerStreamAvailable { get; set; }
}