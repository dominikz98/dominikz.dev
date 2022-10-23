using dominikz.kernel.Contracts;

namespace dominikz.kernel.ViewModels;

public abstract class MediaVM : IViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public FileVM? Image { get; set; }
}

public class MediaPreviewVM : MediaVM
{
    public MediaCategoryEnum Category { get; set; }
}