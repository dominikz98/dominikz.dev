using dominikz.Domain.Filter;
using dominikz.Domain.ViewModels.Songs;

namespace dominikz.Infrastructure.Clients.Api;

public class SongsEndpoints
{
    private readonly ApiClient _client;
    private const string Endpoint = "songs";

    public SongsEndpoints(ApiClient client)
    {
        _client = client;
    }

    public async Task<SongVm?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await _client.GetSingle<SongVm>($"{Endpoint}/{id}", cancellationToken);

    public async Task<IReadOnlyCollection<SongVm>> Search(SongsFilter filter, CancellationToken cancellationToken = default)
        => await _client.Get<SongVm>($"{Endpoint}/search", filter, cancellationToken);
}