using dominikz.shared.Contracts;

namespace dominikz.shared.ViewModels.Media;

public class GameVM : MediaVM
{
    public int Year { get; init; }
    public GamePlatformEnum Platform { get; init; }
    public GameGenresFlags Genres { get; init; }
}
