using dominikz.Domain.Contracts;
using dominikz.Domain.Enums.Media;

namespace dominikz.Domain.Filter;

public class GamesFilter : IFilter
{
    public string? Text { get; init; }
    public GamePlatformEnum? Platform { get; init; }
    public GameGenresFlags? Genres { get; init; }

    public IReadOnlyCollection<FilterParam> GetParameter()
    {
        var result = new List<FilterParam>();

        if (Text is not null)
            result.Add(new(nameof(Text), Text));

        if (Genres is not null && Genres != GameGenresFlags.All)
            result.Add(new(nameof(Genres), Genres.ToString()!));

        if (Platform is not null)
            result.Add(new(nameof(Platform), Platform.ToString()!));

        return result;
    }
}
