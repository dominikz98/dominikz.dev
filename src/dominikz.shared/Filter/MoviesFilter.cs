using dominikz.shared.Contracts;

namespace dominikz.shared.Filter;

public class MoviesFilter : IFilter
{
    public string? Text { get; set; }
    public MovieGenresFlags Genres { get; set; }

    public IReadOnlyCollection<FilterParam> GetParameter()
    {
        var result = new List<FilterParam>();

        if (Text is not null)
            result.Add(new(nameof(Text), Text));

        if (Genres != MovieGenresFlags.ALL)
            result.Add(new(nameof(Genres), Genres.ToString()!));

        return result;
    }
}
