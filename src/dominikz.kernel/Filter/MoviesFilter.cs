using dominikz.kernel.Contracts;

namespace dominikz.kernel.Filter;

public class MoviesFilter : IFilter
{
    public string? Text { get; set; }
    public MovieGenreFlags Genres { get; set; }

    public IReadOnlyCollection<FilterParam> GetParameter()
    {
        var result = new List<FilterParam>();

        if (Text is not null)
            result.Add(new(nameof(Text), Text));

        if (Genres != MovieGenreFlags.ALL)
            result.Add(new(nameof(Genres), Genres.ToString()!));

        return result;
    }
}
