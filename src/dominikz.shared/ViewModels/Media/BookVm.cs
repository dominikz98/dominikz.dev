using dominikz.shared.Enums;

namespace dominikz.shared.ViewModels.Media;

public class BookVm : MediaVm
{
    public int Year { get; init; }
    public string Author { get; init; } = string.Empty;
    public BookLanguageEnum Language { get; init; }
    public BookGenresFlags Genres { get; init; }
}
