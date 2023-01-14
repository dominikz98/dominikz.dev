using dominikz.Domain.Enums;

namespace dominikz.Domain.ViewModels.Media;

public class BookVm : MediaVm
{
    public int Year { get; init; }
    public string Author { get; init; } = string.Empty;
    public BookLanguageEnum Language { get; init; }
    public BookGenresFlags Genres { get; init; }
}
