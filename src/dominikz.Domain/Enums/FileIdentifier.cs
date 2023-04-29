using dominikz.Domain.Enums.Files;
using HeyRed.Mime;

namespace dominikz.Domain.Enums;

public static class FileIdentifier
{
    private static readonly List<FileExtensionEnum> ImageExtensions = new()
    {
        FileExtensionEnum.Jpg, FileExtensionEnum.Jpeg, FileExtensionEnum.Png, FileExtensionEnum.Webp
    };

    public static FileExtensionEnum GetExtensionByName(string filename)
    {
        var extension = Path.GetExtension(filename);
        if (string.IsNullOrWhiteSpace(extension))
            return FileExtensionEnum.Unknown;

        if (Enum.TryParse<FileExtensionEnum>(extension[1..], true, out var result) == false)
            return FileExtensionEnum.Unknown;

        return result;
    }

    public static FileExtensionEnum GetExtensionByContentType(string contentType)
    {
        var extension = MimeTypesMap.GetExtension(contentType);
        return GetExtensionByName($"{Guid.NewGuid()}.{extension}");
    }
    
    public static FileCategoryEnum GetCategoryByName(string filename)
    {
        var extension = GetExtensionByName(filename);
        return GetCategoryByExtension(extension);
    }

    public static FileCategoryEnum GetCategoryByExtension(FileExtensionEnum extension)
    {
        if (ImageExtensions.Contains(extension))
            return FileCategoryEnum.Image;

        return FileCategoryEnum.Unknown;
    }
}