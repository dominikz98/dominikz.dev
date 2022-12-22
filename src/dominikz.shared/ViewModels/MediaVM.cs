using dominikz.shared.Contracts;

namespace dominikz.shared.ViewModels;

public abstract class MediaVM : IViewModel
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public FileVM? Image { get; init; }
}

public class MediaPreviewVM : MediaVM
{
    public MediaCategoryEnum Category { get; init; }
}