namespace dominikz.api.Provider;

public interface IStorageProvider
{
    Task Upload(Guid id, Stream source, CancellationToken cancellationToken);
    Task<Stream?> Download(Guid id, CancellationToken cancellationToken);
    Task<bool> Exists(Guid id, CancellationToken cancellationToken);
    Task<bool> TryDelete(Guid id, CancellationToken cancellationToken);
    void Delete(Guid id);
    Task SaveChanges(CancellationToken cancellationToken);
}

public class IoStorageProvider : IStorageProvider
{
    private const string StoragePath = "/home/dominikzettl/Pictures/DominikZ";
    private readonly List<TransactionItem> _transaction = new();

    public async Task<Stream?> Download(Guid id, CancellationToken cancellationToken)
    {
        var filepath = Path.Combine(StoragePath, id.ToString());
        await using var fs = new FileStream(filepath, FileMode.Open);
        if (fs.Length == 0)
            return null;
        
        var ms = new MemoryStream();
        await fs.CopyToAsync(ms, cancellationToken);
        ms.Position = 0;

        return ms;
    }

    public async Task<bool> TryDelete(Guid id, CancellationToken cancellationToken)
    {
        var exists = await Exists(id, cancellationToken);
        if (exists == false)
            return false;

        Delete(id);
        return true;
    }

    public void Delete(Guid id)
        => _transaction.Add(new TransactionItem()
        {
            Action = TransactionAction.Delete,
            Timestamp = DateTime.Now,
            Id = id
        });

    public Task<bool> Exists(Guid id, CancellationToken cancellationToken)
    {
        var exists = File.Exists(Path.Combine(StoragePath, id.ToString()));
        exists = exists && _transaction.Where(x => x.Id == id)
            .Where(x => x.Action == TransactionAction.Delete)
            .Any() == false;
        
        return Task.FromResult(exists);
    }

    public async Task Upload(Guid id, Stream source, CancellationToken cancellationToken)
    {
        var ms = new MemoryStream();
        source.Position = 0;
        await source.CopyToAsync(ms, cancellationToken);
        source.Position = 0;
        ms.Position = 0;
        
        _transaction.Add(new TransactionItem()
        {
            Id = id,
            Action = TransactionAction.Upload,
            Timestamp = DateTime.Now,
            Data = ms
        });
    }

    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        foreach (var item in _transaction.OrderBy(x => x.Timestamp))
        {
            if (item.Action == TransactionAction.Upload)
            {
                var filepath = Path.Combine(StoragePath, item.Id.ToString());
                await using var fs = new FileStream(filepath, FileMode.OpenOrCreate);
                
                item.Data!.Position = 0;
                await item.Data!.CopyToAsync(fs, cancellationToken);
                item.Data!.Position = 0;
            }
            else
            {
                File.Delete(Path.Combine(StoragePath, item.Id.ToString()));
            }
        }
    }

    class TransactionItem
    {
        public DateTime Timestamp { get; init; }
        public TransactionAction Action { get; init; }
        public Stream? Data { get; init; }
        public Guid Id { get; init; }
    }

    enum TransactionAction
    {
        Upload,
        Delete
    }
}