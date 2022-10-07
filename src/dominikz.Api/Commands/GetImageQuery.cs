using dominikz.Api.Provider;
using MediatR;

namespace dominikz.Api.Commands;

public class GetImageQuery : IRequest<Stream?>
{
    public Guid Id { get; set; }

    public GetImageQuery(Guid id)
    {
        Id = id;
    }
}

public class GetImageQueryHandler : IRequestHandler<GetImageQuery, Stream?>
{
    private readonly IStorageProvider _storage;

    public GetImageQueryHandler(IStorageProvider storage)
    {
        _storage = storage;
    }

    public async Task<Stream?> Handle(GetImageQuery request, CancellationToken cancellationToken)
        => await _storage.Load(request.Id, cancellationToken);
}