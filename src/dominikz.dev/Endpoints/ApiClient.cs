using System.Net.Http.Json;
using System.Text.Json;
using dominikz.dev.Components.Toast;
using dominikz.shared.Contracts;

namespace dominikz.dev.Endpoints;

public class ApiClient
{
    private readonly HttpClient _client;
    private readonly ToastService _toast;

    public const string ApiKeyHeaderName = "x-api-key";
    public const string Prefix = "api";

    private readonly JsonSerializerOptions _serializerOptions;

    public ApiClient(HttpClient client, ToastService toast)
    {
        _client = client;
        _toast = toast;
        _serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, };
    }

    public async Task<T?> GetSingle<T>(string endpoint, CancellationToken cancellationToken) where T : new()
    {
        try
        {
            var route = CreateEndpointUrl(endpoint);
            return await _client.GetFromJsonAsync<T>(route, _serializerOptions, cancellationToken);
        }
        catch (Exception)
        {
            _toast.Show($"Error calling '{endpoint}'", ToastLevel.Error);
            return default;
        }
    }

    public async Task<List<T>> Get<T>(string endpoint, CancellationToken cancellationToken) where T : new()
        => await Get<T>(endpoint, null, cancellationToken);

    public async Task<List<T>> Get<T>(string endpoint, IFilter? filter, CancellationToken cancellationToken) where T : new()
    {
        try
        {
            var route = CreateEndpointUrl(endpoint, filter);
            var result = await _client.GetFromJsonAsync<List<T>>(route, _serializerOptions, cancellationToken);
            return result ?? new List<T>();
        }
        catch (Exception)
        {
            _toast.Show($"Error calling '{endpoint}'", ToastLevel.Error);
            return new List<T>();
        }
    }

    public async Task<TResponse?> Post<TRequest, TResponse>(string endpoint, TRequest data, bool suppressToast, CancellationToken cancellationToken) where TRequest : new() where TResponse : new()
    {
        try
        {
            var route = CreateEndpointUrl(endpoint);
            var result = await _client.PostAsJsonAsync(route, data, _serializerOptions, cancellationToken);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadFromJsonAsync<TResponse>(_serializerOptions, cancellationToken);
        }
        catch (Exception)
        {
            if (suppressToast == false)
                _toast.Show($"Error calling '{endpoint}'", ToastLevel.Error);
            return default;
        }
    }

    public void SetAuthHeader(string token)
        => _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

    public void RemoveAuthHeader()
        => _client.DefaultRequestHeaders.Remove("Authorization");
    
    public string Curl(string endpoint, IFilter? filter)
    {
        var route = CreateEndpointUrl(endpoint, filter);
        return $"curl {_client.BaseAddress}{route}";
    }

    private static string CreateEndpointUrl(string endpoint, IFilter? filter = null)
    {
        var route = $"{Prefix}/{endpoint}";

        var parameter = filter?.GetParameter()
            .Select(x => $"{x.Name}={x.Value}")
            .ToList() ?? new List<string>();

        if (parameter.Count > 0)
        {
            var query = string.Join('&', parameter);
            route += $"?{query}";
        }

        return route;
    }
}