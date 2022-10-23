using dominikz.api.Models;
using dominikz.kernel.ViewModels;

namespace dominikz.api.Mapper;

public static class MediaMapper
{
    public static IQueryable<MediaPreviewVM> MapToVM(this IQueryable<Media> query)
        => query.Select(media => new MediaPreviewVM()
        {
            Id = media.Id,
            Title = media.Title,
            Timestamp = media.Timestamp,
            Image = media.File!.MapToVM(),
            Category = media.Category
        });
}
