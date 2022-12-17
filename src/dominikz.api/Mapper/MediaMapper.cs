using dominikz.api.Models;
using dominikz.shared.ViewModels;

namespace dominikz.api.Mapper;

public static class MediaMapper
{
    public static IQueryable<MediaPreviewVM> MapToPreviewVm(this IQueryable<Media> query)
        => query.Select(media => new MediaPreviewVM()
        {
            Id = media.Id,
            Title = media.Title,
            Timestamp = media.Timestamp,
            Image = media.File!.MapToVm(),
            Category = media.Category
        });
}
