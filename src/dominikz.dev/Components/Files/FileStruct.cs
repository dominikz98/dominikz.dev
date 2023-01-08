using dominikz.shared;
using dominikz.shared.Enums;
using HeyRed.Mime;

namespace dominikz.dev.Components.Files;

public struct FileStruct
{
    public string Name { get; }
    public Stream Data { get; }
    public string DataAsPath { get; }

    private FileExtensionEnum _extension;

    public FileStruct(string name, Stream data)
    {
        _extension = FileIdentifier.GetExtensionByName(name);
        Name = name;
        Data = data;
        var contentType = MimeTypesMap.GetMimeType(Name);
        DataAsPath = PopulateImageFromStream(Data, contentType);
        Data.Position = 0;
    }

    public FileStruct(string nameWithoutExt, string contentType, Stream data)
    {
        _extension = FileIdentifier.GetExtensionByContentType(contentType);
        Name = $"{nameWithoutExt}.{_extension}";
        Data = data;
        DataAsPath = PopulateImageFromStream(Data, contentType);
        Data.Position = 0;
    }

    public FileStruct CoptyTo(string nameWithoutExt)
    {
        Data.Position = 0;
        return new($"{nameWithoutExt}.{_extension}", Data);
    }

    private string PopulateImageFromStream(Stream stream, string contentType)
    {
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        var bytes = ms.ToArray();
        var b64String = Convert.ToBase64String(bytes);
        return $"data:{contentType};base64,{b64String}";
    }
}