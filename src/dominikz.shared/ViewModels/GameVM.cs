using dominikz.shared.Contracts;

namespace dominikz.shared.ViewModels;

public class GameVM : MediaVM
{
    public int Year { get; set; }
    public GamePlatformEnum Platform { get; set; }
    public GameGenresFlags Genres { get; set; }
}
