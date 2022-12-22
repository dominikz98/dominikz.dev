using dominikz.shared.Contracts;

namespace dominikz.shared.Filter;

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

        if (Genres is null || Genres != BookGenresFlags.ALL)
            result.Add(new(nameof(Genres), Genres.ToString()!));

        if (Language is not null)
            result.Add(new(nameof(Language), Language.ToString()!));

        return result;
    }
}
