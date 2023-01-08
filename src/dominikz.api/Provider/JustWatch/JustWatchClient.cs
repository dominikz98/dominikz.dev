namespace dominikz.api.Provider.JustWatch;

public class JustWatchClient
{
    private readonly HttpClient _client;
    private const string Local = "en_US";

    public JustWatchClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<JustWatchMovieVm?> GetMovieById(int id, CancellationToken cancellationToken)
        => await _client.GetFromJsonAsync<JustWatchMovieVm>($"content/titles/movie/{id}/locale/{Local}", cancellationToken);

    public async Task<int?> SearchMovieByName(string name, CancellationToken cancellationToken)
    {
        var jwParams = new JustWatchSearchParamsVm
        {
            Query = name
        };
        jwParams.ContentTypes.Add("movie");

        var response = await _client.PostAsJsonAsync($"content/titles/{Local}/popular", jwParams, cancellationToken);
        if (response.IsSuccessStatusCode == false)
            return null;

        var data = await response.Content.ReadFromJsonAsync<JustWatchSearchResultVm>(cancellationToken: cancellationToken);
        var idRaw = data?.Items.FirstOrDefault(x => x.Title.Equals(name, StringComparison.OrdinalIgnoreCase))?.Id;
        if (int.TryParse(idRaw.ToString(), out var id) == false)
            return null;

        return id;
    }
}