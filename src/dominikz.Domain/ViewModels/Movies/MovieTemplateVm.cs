namespace dominikz.Domain.ViewModels.Movies;

public class MovieTemplateVm
{
    public string ImdbId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<string> GenreRecommendations { get; set; } = new();
    public int Rating { get; set; }
    public int Year { get; set; }
    public string Plot { get; set; } = string.Empty;
    public TimeSpan Runtime { get; set; }
    public List<string> PosterUrls { get; set; } = new();
}