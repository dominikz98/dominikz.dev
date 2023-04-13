using dominikz.Domain.Contracts;
using dominikz.Domain.Enums.Movies;

namespace dominikz.Domain.ViewModels.Movies;

public class MovieVm : IHasImageUrl
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public DateTime? PublishDate { get; init; }
    public MovieGenresFlags Genres { get; init; }
    public string ImageUrl { get; set; } = string.Empty;
    public int Rating { get; init; }
    public int Year { get; init; }
}