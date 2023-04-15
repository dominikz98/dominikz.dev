namespace dominikz.Infrastructure.Provider.Storage.Requests;

public interface IStorageProcessor
{
    Task<Stream> Execute(Stream data, CancellationToken cancellationToken);
}

public interface IStorageDownloadRequest : IStorageRequest
{
    bool UseAsync { get; }
    IReadOnlyCollection<IStorageProcessor> Processors { get; }
}

public interface IStorageUploadRequest : IStorageRequest
{
    Stream Data { get; }
    IReadOnlyCollection<IStorageProcessor> Processors { get; }
}

public interface IStorageRequest
{
    string Name { get; }
}