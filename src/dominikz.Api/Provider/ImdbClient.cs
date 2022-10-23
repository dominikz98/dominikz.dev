using dominikz.kernel.ViewModels;

namespace dominikz.api.Provider;

public class ImdbClient
{
    private readonly HttpClient _client;

    public ImdbClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<ImdbVM?> GetById(string id, CancellationToken cancellationToken)

        => await _client.GetFromJsonAsync<ImdbVM>($"title/{id}", cancellationToken);
}
