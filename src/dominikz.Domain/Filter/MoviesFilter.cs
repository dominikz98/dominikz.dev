using dominikz.Domain.Contracts;
using dominikz.Domain.Enums.Media;

namespace dominikz.Domain.Filter;

public class MoviesFilter : IFilter
{
    public string? Text { get; init; }
    public MovieGenresFlags? Genres { get; init; }

    public IReadOnlyCollection<FilterParam> GetParameter()
    {
        var result = new List<FilterParam>();

        if (Text is not null)
            result.Add(new(nameof(Text), Text));

        if (Genres is not null && Genres != MovieGenresFlags.All)
            result.Add(new(nameof(Genres), Genres.ToString()!));

        return result;
    }
}
