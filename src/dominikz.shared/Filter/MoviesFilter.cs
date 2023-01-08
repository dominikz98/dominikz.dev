using dominikz.shared.Contracts;
using dominikz.shared.Enums;

namespace dominikz.shared.Filter;

public class MoviesFilter : IFilter
{
    public string? Text { get; init; }
    public MovieGenresFlags? Genres { get; init; }

    public IReadOnlyCollection<FilterParam> GetParameter()
    {
        var result = new List<FilterParam>();

        if (Text is not null)
            result.Add(new(nameof(Text), Text));

        if (Genres is not null && Genres != MovieGenresFlags.ALL)
            result.Add(new(nameof(Genres), Genres.ToString()!));

        return result;
    }
}
