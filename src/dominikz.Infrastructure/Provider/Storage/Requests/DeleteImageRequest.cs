namespace dominikz.Infrastructure.Provider.Storage.Requests;

public class DeleteImageRequest : IStorageRequest
{
    public string Name { get; }

    public DeleteImageRequest(Guid id)
    {
        Name = Path.Combine("images", $"{id}.jpg".ToLower());
    }
}