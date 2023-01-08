using dominikz.shared.Enums;

namespace dominikz.shared.ViewModels.Media;

public class MovieVm : MediaVm
{
    public MovieGenresFlags Genres { get; init; }
    public int Rating { get; init; }
    public int Year { get; init; }
}