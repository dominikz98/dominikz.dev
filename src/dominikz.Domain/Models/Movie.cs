using dominikz.Domain.Enums;
using dominikz.Domain.Enums.Media;

namespace dominikz.Domain.Models;

public class Movie : Media
{
    public string? Comment { get; set; }
    public MovieGenresFlags Genres { get; set; }
    public int Rating { get; set; }
    public int Year { get; set; }
    public string Plot { get; set; } = string.Empty;
    public TimeSpan Runtime { get; set; }
    public string ImdbId { get; set; } = string.Empty;
    public string YoutubeId { get; set; } = string.Empty;
    public int JustWatchId { get; set; }

    public List<MoviesPersonsMapping> MoviesPersonsMappings { get; set; } = new();
}

public class MoviesPersonsMapping
{
    public Guid MovieId { get; set; }
    public Guid PersonId { get; set; }
    public PersonCategoryFlags Category { get; set; }

    public Movie? Movie { get; set; }
    public Person? Person { get; set; }
}
