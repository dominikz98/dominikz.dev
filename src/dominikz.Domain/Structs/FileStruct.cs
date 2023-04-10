
namespace dominikz.Domain.Structs;

public struct FileStruct
{
    public string Name { get; }
    public Stream Data { get; }
    public string DataAsPath { get; }

    private readonly string _contentType;

    public FileStruct(string name, string contentType, Stream data)
    {
        Name = name;
        Data = data;
        _contentType = contentType;
        DataAsPath = PopulateImageFromStream(Data, _contentType);
        Data.Position = 0;
    }

    public FileStruct CopyTo(string name)
    {
        Data.Position = 0;
        return new(name, _contentType, Data);
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