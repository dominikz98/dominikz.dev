namespace dominikz.shared.ViewModels.Media;

public class MovieTemplateVm
{
    public string ImdbId { get; set; } = string.Empty;
    public int JustWatchId { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<string> GenreRecommendations { get; set; } = new();
    public int Rating { get; set; }
    public int Year { get; set; }
    public string Plot { get; set; } = string.Empty;
    public TimeSpan Runtime { get; set; }
    public string YouTubeId { get; set; } = string.Empty;
    public List<string> PosterUrls { get; set; } = new();

    public List<EditPersonVm> Directors { get; set; } = new();
    public List<EditPersonVm> Writers { get; set; } = new();
    public List<EditPersonVm> Stars { get; set; } = new();
}