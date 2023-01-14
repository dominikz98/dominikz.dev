using dominikz.Domain.Enums;

namespace dominikz.Domain.Models;

public class Game : Media
{
    public int Year { get; set; }
    public GamePlatformEnum Platform { get; set; }
    public GameGenresFlags Genres { get; set; }
}
