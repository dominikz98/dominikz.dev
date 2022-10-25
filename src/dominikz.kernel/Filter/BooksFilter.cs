using dominikz.kernel.Contracts;

namespace dominikz.kernel.Filter;

public class BooksFilter : IFilter
{
    public string? Text { get; set; }
    public BookLanguageEnum? Language { get; set; }
    public BookGenresFlags Genres { get; set; }

    public IReadOnlyCollection<FilterParam> GetParameter()
    {
        var result = new List<FilterParam>();

        if (Text is not null)
            result.Add(new(nameof(Text), Text));

        if (Genres != BookGenresFlags.ALL)
            result.Add(new(nameof(Genres), Genres.ToString()!));

        if (Language is not null)
            result.Add(new(nameof(Language), Language.ToString()!));

        return result;
    }
}
