namespace dominikz.Infrastructure.Provider.Storage;

public interface IStorageProvider
{
    Task Upload(IStorageUploadRequest request, CancellationToken cancellationToken);
    Task<Stream?> Download(IStorageDownloadRequest request, CancellationToken cancellationToken);
    Task<bool> Exists(IStorageRequest request, CancellationToken cancellationToken);
    Task<bool> TryDelete(IStorageRequest request, CancellationToken cancellationToken);
    void Delete(IStorageRequest request);
}

public class StorageProvider : IStorageProvider
{
    private readonly string _rootPath;

    public StorageProvider(string? rootPath)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
            throw new ArgumentException("Invalid storage path!");

        _rootPath = rootPath;
    }

    public async Task Upload(IStorageUploadRequest request, CancellationToken cancellationToken)
    {
        var data = request.Data;
        foreach (var processor in request.Processors)
            data = await processor.Execute(data, cancellationToken);

        var path = Path.Combine(_rootPath, request.Name);
        await using var fs = new FileStream(path, FileMode.OpenOrCreate);
        data.Position = 0;
        await data.CopyToAsync(fs, cancellationToken);
        data.Position = 0;
    }

    public async Task<Stream?> Download(IStorageDownloadRequest request, CancellationToken cancellationToken)
    {
        FileStream fs;
        var path = Path.Combine(_rootPath, request.Name);
        fs = File.OpenRead(path);

        if (fs.Length == 0)
            return null;

        if (request.Processors.Count == 0)
            return fs;

        Stream result = fs;
        foreach (var processor in request.Processors)
            result = await processor.Execute(result, cancellationToken);

        await fs.DisposeAsync();
        return result;
    }

    public Task<bool> Exists(IStorageRequest request, CancellationToken cancellationToken)
    {
        var path = Path.Combine(_rootPath, request.Name);
        var exists = File.Exists(path);
        return Task.FromResult(exists);
    }

    public Task<bool> TryDelete(IStorageRequest request, CancellationToken cancellationToken)
    {
        var path = Path.Combine(_rootPath, request.Name);
        var exists = File.Exists(path);
        if (exists)
            Delete(request);

        return Task.FromResult(exists);
    }

    public void Delete(IStorageRequest request)
    {
        var path = Path.Combine(_rootPath, request.Name);
        File.Delete(path);
    }
}