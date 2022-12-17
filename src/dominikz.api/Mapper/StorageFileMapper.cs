using dominikz.api.Models;
using dominikz.shared.ViewModels;

namespace dominikz.api.Mapper;

public static class StorageFileMapper
{
    public static IQueryable<FileVM> MapToVm(this IQueryable<StorageFile> query)
        => query.Select(file => file.MapToVm());

    public static FileVM MapToVm(this StorageFile file)
        => new()
        {
            Id = file.Id,
            Category = file.Category,
            Extension = file.Extension
        };
}
