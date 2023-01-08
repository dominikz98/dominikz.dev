namespace dominikz.shared.Enums;

[Flags]
public enum BookGenresFlags
{
    ALL = 0,
    Crime = 1,
    Fantasy = 2,
    Adventure = 4,
    Horror = 8,
    SciFi = 16,
    Novel = 32,
    Thriller = 64,
    Dystopia = 128,
    NonFiction = 256,
    Advising = 512,
    Romance = 1024,
    Humor = 2048
}
