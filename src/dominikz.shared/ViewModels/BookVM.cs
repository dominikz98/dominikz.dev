using dominikz.shared.Contracts;

namespace dominikz.shared.ViewModels;

public class BookVM : MediaVM
{
    public int Year { get; set; }
    public string Author { get; set; } = string.Empty;
    public BookLanguageEnum Language { get; set; }
    public BookGenresFlags Genres { get; set; }
}
