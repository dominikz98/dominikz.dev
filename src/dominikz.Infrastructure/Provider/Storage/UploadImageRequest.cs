namespace dominikz.Infrastructure.Provider.Storage;

public class UploadImageRequest : IStorageUploadRequest
{
    public string Name { get; }
    public Stream Data { get; }

    public IReadOnlyCollection<IStorageProcessor> Processors { get; } = new[]
    {
        new ConvertImageToJpgProcessor()
    };

    public UploadImageRequest(Guid id, Stream data)
    {
        Name = Path.Combine("images", $"{id}.jpg".ToLower());
        Data = data;
    }
}