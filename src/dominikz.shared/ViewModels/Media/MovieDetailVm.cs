namespace dominikz.shared.ViewModels.Media;

public class MovieDetailVm : MovieVm
{
    public string YoutubeId { get; init; } = string.Empty;
    public string? Comment { get; init; }
    public string Plot { get; init; } = string.Empty;
    public TimeSpan Runtime { get; init; }
    public string AuthorImageUrl { get; set; } = string.Empty;
    public List<PersonVm> Directors { get; init; } = new();
    public List<PersonVm> Writers { get; init; } = new();
    public List<PersonVm> Stars { get; init; } = new();
}