using dominikz.shared.Contracts;

namespace dominikz.shared.ViewModels.Media;

public abstract class MediaVM : IViewModel, IHasImageUrl
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public string ImageUrl { get; set; } = string.Empty;
}

public class MediaPreviewVM : MediaVM
{
    public MediaCategoryEnum Category { get; init; }
}