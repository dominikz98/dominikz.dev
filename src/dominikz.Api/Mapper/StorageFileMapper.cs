using dominikz.api.Models;
using dominikz.kernel.ViewModels;

namespace dominikz.api.Mapper;

public static class StorageFileMapper
{
    public static IQueryable<FileVM> MapToVM(this IQueryable<StorageFile> query)
        => query.Select(file => file.MapToVM());

    public static FileVM MapToVM(this StorageFile file)
        => new()
        {
            Id = file.Id,
            Category = file.Category,
            Extension = file.Extension
        };
}
