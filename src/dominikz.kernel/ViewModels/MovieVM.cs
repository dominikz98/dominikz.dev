using dominikz.kernel.Contracts;

namespace dominikz.kernel.ViewModels;

public class MovieVM : MediaVM
{
    public MovieGenresFlags Genres { get; set; } = new();
    public int Rating { get; set; }
    public int Year { get; set; }
}

public class MovieDetailVM : MovieVM
{
    public string YoutubeId { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public PersonVM? Author { get; set; }
    public string Plot { get; set; } = string.Empty;
    public TimeSpan Runtime { get; set; }
    public List<PersonVM> Directors { get; set; } = new();
    public List<PersonVM> Writers { get; set; } = new();
    public List<PersonVM> Stars { get; set; } = new();
}
