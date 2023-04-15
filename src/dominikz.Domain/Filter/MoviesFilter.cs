using dominikz.Domain.Contracts;
using dominikz.Domain.Enums.Movies;

namespace dominikz.Domain.Filter;

public class MoviesFilter : IFilter
{
    public int? Start { get; set; }
    public int? Count { get; set; }
    public string? Text { get; init; }
    public MovieGenresFlags? Genres { get; init; }

    public IReadOnlyCollection<FilterParam> GetParameter()
    {
        var result = new List<FilterParam>();

        if (Start is not null)
            result.Add(new(nameof(Start), Start.Value.ToString()));
        
        if (Count is not null)
            result.Add(new(nameof(Count), Count.Value.ToString()));
        
        if (string.IsNullOrWhiteSpace(Text) == false)
            result.Add(new(nameof(Text), Text));

        if (Genres is not null && Genres != MovieGenresFlags.All)
            result.Add(new(nameof(Genres), Genres.ToString()!));

        return result;
    }
}