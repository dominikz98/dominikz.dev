using dominikz.Domain.Contracts;
using dominikz.Domain.Enums.Media;

namespace dominikz.Domain.Filter;

public class BooksFilter : IFilter
{
    public string? Text { get; init; }
    public BookLanguageEnum? Language { get; init; }
    public BookGenresFlags? Genres { get; init; }

    public IReadOnlyCollection<FilterParam> GetParameter()
    {
        var result = new List<FilterParam>();

        if (Text is not null)
            result.Add(new(nameof(Text), Text));

        if (Genres is not null && Genres != BookGenresFlags.All)
            result.Add(new(nameof(Genres), Genres.ToString()!));

        if (Language is not null)
            result.Add(new(nameof(Language), Language.ToString()!));

        return result;
    }
}
