using dominikz.Domain.Enums.Media;

namespace dominikz.Domain.ViewModels.Media;

public class GameVm : MediaVm
{
    public int Year { get; init; }
    public GamePlatformEnum Platform { get; init; }
    public GameGenresFlags Genres { get; init; }
}
