using dominikz.Domain.Enums;
using dominikz.Domain.Enums.Media;

namespace dominikz.Domain.ViewModels.Media;

public class MovieVm : MediaVm
{
    public MovieGenresFlags Genres { get; init; }
    public int Rating { get; init; }
    public int Year { get; init; }
}