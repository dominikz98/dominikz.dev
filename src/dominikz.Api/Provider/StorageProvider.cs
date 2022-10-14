namespace dominikz.api.Provider;

public interface IStorageProvider
{
    Task<Stream?> Load(Guid id, CancellationToken cancellationToken);
}

public class StorageProvider : IStorageProvider
{
    private const string _storagePath = @"C:\Users\Dominik\Pictures\DominikZ";

    public async Task<Stream?> Load(Guid id, CancellationToken cancellationToken)
    {
        var filepath = Path.Combine(_storagePath, id.ToString());
        using var fs = new FileStream(filepath, FileMode.Open);

        var ms = new MemoryStream();
        await fs.CopyToAsync(ms, cancellationToken);
        ms.Position = 0;

        return ms;
    }

    public async Task Save(Guid id, Stream source, CancellationToken cancellationToken)
    {
        var filepath = Path.Combine(_storagePath, id.ToString());
        using var fs = new FileStream(filepath, FileMode.CreateNew);
        await source.CopyToAsync(fs, cancellationToken);
        source.Position = 0;
    }
}
