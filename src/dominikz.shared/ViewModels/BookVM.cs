using dominikz.shared.Contracts;

namespace dominikz.shared.ViewModels;

public class BookVM : MediaVM
{
    public int Year { get; init; }
    public string Author { get; init; } = string.Empty;
    public BookLanguageEnum Language { get; init; }
    public BookGenresFlags Genres { get; init; }
}
