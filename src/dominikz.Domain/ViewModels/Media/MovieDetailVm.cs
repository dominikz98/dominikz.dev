namespace dominikz.Domain.ViewModels.Media;

public class MovieDetailVm : MovieVm
{
    public string YoutubeId { get; init; } = string.Empty;
    public string? Comment { get; init; }
    public string Plot { get; init; } = string.Empty;
    public TimeSpan Runtime { get; init; }
    public bool IsStreamable { get; set; }
}