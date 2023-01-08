using dominikz.api.Models;
using dominikz.shared.ViewModels.Media;

namespace dominikz.api.Mapper;

public static class MediaMapper
{
    public static IQueryable<MediaPreviewVm> MapToPreviewVm(this IQueryable<Media> query)
        => query.Select(media => new MediaPreviewVm()
        {
            Id = media.Id,
            Title = media.Title,
            PublishDate = media.PublishDate,
            ImageUrl = media.File!.Id.ToString(),
            Category = media.Category
        });
}
