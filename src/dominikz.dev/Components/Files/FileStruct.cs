using dominikz.shared;
using HeyRed.Mime;

namespace dominikz.dev.Components.Files;

public struct FileStruct
{
    public string Name { get; }
    public Stream Data { get; }
    public string DataAsPath { get; }
    
    public FileStruct(string name, Stream data)
    {
        Name = name;
        Data = data;
        var contentType = MimeTypesMap.GetMimeType(Name);
        DataAsPath = PopulateImageFromStream(Data, contentType);
    }
    
    public FileStruct(string nameWithoutExt, string contentType, Stream data)
    {
        Name = $"{nameWithoutExt}.{FileIdentifier.GetExtensionByContentType(contentType)}";
        Data = data;
        DataAsPath = PopulateImageFromStream(Data, contentType);
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