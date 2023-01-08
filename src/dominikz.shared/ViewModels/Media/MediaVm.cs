using dominikz.shared.Contracts;
using dominikz.shared.Enums;

namespace dominikz.shared.ViewModels.Media;

public abstract class MediaVm : IHasImageUrl
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public DateTime? PublishDate { get; init; }
    public string ImageUrl { get; set; } = string.Empty;
}

public class MediaPreviewVm : MediaVm
{
    public MediaCategoryEnum Category { get; init; }
}