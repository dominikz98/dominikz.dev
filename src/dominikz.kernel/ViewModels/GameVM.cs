using dominikz.kernel.Contracts;

namespace dominikz.kernel.ViewModels;

public class GameVM : MediaVM
{
    public int Year { get; set; }
    public GamePlatformEnum Platform { get; set; }
    public GameGenresFlags Genres { get; set; }
}
