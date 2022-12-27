using dominikz.shared.ViewModels;

namespace dominikz.dev.Endpoints;

public class MusicEndpoints
{
    private readonly ApiClient _client;
    private const string Endpoint = "music";

    public MusicEndpoints(ApiClient client)
    {
        _client = client;
    }

    public async Task<SongVm?> GetSongById(Guid id, CancellationToken cancellationToken = default)
        => await _client.GetSingle<SongVm>($"{Endpoint}/songs/{id}", cancellationToken);
}
