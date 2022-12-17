using System.Net.Http.Json;
using System.Text.Json;
using dominikz.shared.Contracts;

namespace dominikz.dev.Endpoints;

public class ApiClient
{
    private readonly HttpClient _client;

    private static readonly string _prefix = "api";

    private readonly JsonSerializerOptions _serializerOptions;

    public ApiClient(HttpClient client)
    {
        _client = client;
        _serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, };
    }

    public async Task<T?> Get<T>(string endpoint, Guid id, CancellationToken cancellationToken) where T : new()
        => await _client.GetFromJsonAsync<T>($"{_prefix}/{endpoint}/{id}", _serializerOptions, cancellationToken);

    public async Task<List<T>> Get<T>(string endpoint, CancellationToken cancellationToken) where T : new()
    {
        var result = await _client.GetFromJsonAsync<List<T>>($"{_prefix}/{endpoint}", _serializerOptions, cancellationToken);
        return result ?? new List<T>();
    }

    public async Task<List<T>> Get<T>(string endpoint, IFilter? filter, CancellationToken cancellationToken) where T : new()
    {
        var route = $"{_prefix}/{endpoint}";

        var parameter = filter?.GetParameter()
                .Select(x => $"{x.Name}={x.Value}")
                .ToList() ?? new List<string>();

        if (parameter.Count > 0)
        {
            var query = string.Join('&', parameter);
            route += $"?{query}";
        }

        var result = await _client.GetFromJsonAsync<List<T>>(route, _serializerOptions, cancellationToken);
        return result ?? new List<T>();
    }
}
