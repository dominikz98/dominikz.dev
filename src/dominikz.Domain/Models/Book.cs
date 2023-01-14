using dominikz.Domain.Enums;

namespace dominikz.Domain.Models;

public class Book : Media
{
    public int Year { get; set; }
    public string Author { get; set; } = string.Empty;
    public BookLanguageEnum Language { get; set; }
    public BookGenresFlags Genres { get; set; }
}
