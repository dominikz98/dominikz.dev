using dominikz.shared.Contracts;

namespace dominikz.shared.ViewModels.Media;

public class MovieVM : MediaVM
{
    public MovieGenresFlags Genres { get; init; } = new();
    public int Rating { get; init; }
    public int Year { get; init; }
}

public class MovieDetailVM : MovieVM
{
    public string YoutubeId { get; init; } = string.Empty;
    public string? Comment { get; init; }
    public PersonVM? Author { get; init; }
    public string Plot { get; init; } = string.Empty;
    public TimeSpan Runtime { get; init; }
    public List<PersonVM> Directors { get; init; } = new();
    public List<PersonVM> Writers { get; init; } = new();
    public List<PersonVM> Stars { get; init; } = new();
}
