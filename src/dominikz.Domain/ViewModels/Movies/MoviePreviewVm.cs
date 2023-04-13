using dominikz.Domain.Contracts;

namespace dominikz.Domain.ViewModels.Movies;

public class MoviePreviewVm : IHasImageUrl

{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public DateTime? PublishDate { get; init; }
    public string ImageUrl { get; set; } = string.Empty;
}