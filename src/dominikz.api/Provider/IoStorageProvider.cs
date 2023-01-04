namespace dominikz.api.Provider;

public interface IStorageProvider
{
    Task Upload(Guid id, Stream source, CancellationToken cancellationToken);
    Task<Stream?> Download(Guid id, CancellationToken cancellationToken);
    Task Delete(Guid id, CancellationToken cancellationToken);
}

public class IoStorageProvider : IStorageProvider
{
    private const string StoragePath = "/home/dominikzettl/Pictures/DominikZ";

    public async Task<Stream?> Download(Guid id, CancellationToken cancellationToken)
    {
        var filepath = Path.Combine(StoragePath, id.ToString());
        await using var fs = new FileStream(filepath, FileMode.Open);

        var ms = new MemoryStream();
        await fs.CopyToAsync(ms, cancellationToken);
        ms.Position = 0;

        return ms;
    }

    public Task Delete(Guid id, CancellationToken cancellationToken)
    {
        File.Delete(Path.Combine(StoragePath, id.ToString()));
        return Task.CompletedTask;
    }

    public async Task Upload(Guid id, Stream source, CancellationToken cancellationToken)
    {
        var filepath = Path.Combine(StoragePath, id.ToString());
        await using var fs = new FileStream(filepath, FileMode.CreateNew);
        await source.CopyToAsync(fs, cancellationToken);
        source.Position = 0;
    }
}
