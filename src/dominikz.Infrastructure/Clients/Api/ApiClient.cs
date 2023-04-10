using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using dominikz.Domain.Contracts;
using dominikz.Domain.Structs;
using dominikz.Domain.ViewModels.Auth;
using HeyRed.Mime;

namespace dominikz.Infrastructure.Clients.Api;

public class ApiClient
{
    private readonly HttpClient _client;
    private readonly IHttpErrorHandler _errorHandler;
    private readonly ICredentialStorage _credentials;

    public string BaseUrl => _client.BaseAddress?.ToString() ?? string.Empty;
    public const string ApiKeyHeaderName = "x-api-key";
    public const string Prefix = "api";

    private readonly JsonSerializerOptions _serializerOptions;

    public ApiClient(HttpClient client, IHttpErrorHandler errorHandler, ICredentialStorage credentials)
    {
        _client = client;
        _errorHandler = errorHandler;
        _credentials = credentials;
        _serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, };
    }

    public async Task<T?> GetSingle<T>(string endpoint, CancellationToken cancellationToken)
    {
        await AttachOrRefreshTokenIfPossible(cancellationToken);

        try
        {
            var route = CreateEndpointUrl(endpoint);
            return await _client.GetFromJsonAsync<T>(route, _serializerOptions, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            await _errorHandler.Handle(ex.StatusCode, ex.Message, cancellationToken);
            return default;
        }
    }

    public async Task<List<T>> Get<T>(string endpoint, CancellationToken cancellationToken)
        => await Get<T>(endpoint, null, cancellationToken);

    public async Task<List<T>> Get<T>(string endpoint, IFilter? filter, CancellationToken cancellationToken)
    {
        await AttachOrRefreshTokenIfPossible(cancellationToken);

        try
        {
            var route = CreateEndpointUrl(endpoint, filter);
            var result = await _client.GetFromJsonAsync<List<T>>(route, _serializerOptions, cancellationToken);
            return result ?? new List<T>();
        }
        catch (HttpRequestException ex)
        {
            await _errorHandler.Handle(ex.StatusCode, ex.Message, cancellationToken);
            return new List<T>();
        }
    }

    public async Task<TResponse?> Put<TRequest, TResponse>(string endpoint, TRequest data, bool suppressTokenCheck, CancellationToken cancellationToken)
    {
        if (suppressTokenCheck == false)
            await AttachOrRefreshTokenIfPossible(cancellationToken);

        try
        {
            var route = CreateEndpointUrl(endpoint);
            var result = await _client.PutAsJsonAsync(route, data, _serializerOptions, cancellationToken);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadFromJsonAsync<TResponse>(_serializerOptions, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            await _errorHandler.Handle(ex.StatusCode, ex.Message, cancellationToken);
            return default;
        }
    }

    public async Task<TResponse?> Post<TRequest, TResponse>(string endpoint, TRequest data, bool suppressTokenCheck, CancellationToken cancellationToken)
    {
        if (suppressTokenCheck == false)
            await AttachOrRefreshTokenIfPossible(cancellationToken);

        try
        {
            var route = CreateEndpointUrl(endpoint);
            var result = await _client.PostAsJsonAsync(route, data, _serializerOptions, cancellationToken);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadFromJsonAsync<TResponse>(_serializerOptions, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            await _errorHandler.Handle(ex.StatusCode, ex.Message, cancellationToken);
            return default;
        }
    }

    public async Task<TResponse?> Upload<TRequest, TResponse>(HttpMethod method, string endpoint, TRequest data, List<FileStruct> files, CancellationToken cancellationToken)
    {
        await AttachOrRefreshTokenIfPossible(cancellationToken);

        try
        {
            var route = CreateEndpointUrl(endpoint);
            using var request = new HttpRequestMessage(method, route);
            using var content = new MultipartFormDataContent();

            // attach viewmodel
            content.Add(new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"), "ViewModel");

            // attach files
            foreach (var file in files)
            {
                file.Data.Position = 0;
                var fileContent = new StreamContent(file.Data);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(MimeTypesMap.GetMimeType(file.Name));
                content.Add(fileContent, "Files", file.Name);
            }

            request.Content = content;
            var result = await _client.SendAsync(request, cancellationToken);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadFromJsonAsync<TResponse>(_serializerOptions, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            await _errorHandler.Handle(ex.StatusCode, ex.Message, cancellationToken);
            return default;
        }
    }

    public async Task<FileStruct?> Download(string endpoint, string nameWithoutExt, CancellationToken cancellationToken)
    {
        await AttachOrRefreshTokenIfPossible(cancellationToken);

        try
        {
            var route = CreateEndpointUrl(endpoint);
            var response = await _client.GetAsync(route, HttpCompletionOption.ResponseContentRead, cancellationToken);
            var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var contentType = response.Content.Headers.ContentType?.ToString();
            if (contentType is null)
                return null;

            return new FileStruct(nameWithoutExt, contentType, stream);
        }
        catch (HttpRequestException ex)
        {
            await _errorHandler.Handle(ex.StatusCode, ex.Message, cancellationToken);
            return null;
        }
    }

    public async Task<bool> Login(LoginVm vm, CancellationToken cancellationToken = default)
    {
        var credentials = await Post<LoginVm, AuthVm>($"auth/login", vm, true, cancellationToken);
        if (credentials == null)
            return false;

        await _credentials.Set(credentials, cancellationToken);
        return true;
    }

    private async Task AttachOrRefreshTokenIfPossible(CancellationToken cancellationToken)
    {
        var isLoggedIn = await _credentials.IsLoggedIn(cancellationToken);
        if (isLoggedIn == false)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            return;
        }

        var token = await _credentials.GetToken(false, cancellationToken);
        if (token != null)
        {
            // valid token stored
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            return;
        }

        var refreshToken = await _credentials.GetRefreshToken(cancellationToken);
        if (refreshToken == null)
            return;

        // valid refresh token stored
        var outdatedToken = await _credentials.GetToken(true, cancellationToken);
        if (outdatedToken == null)
            return;

        var vm = new RefreshVm() { ExpiredToken = outdatedToken, RefreshToken = refreshToken };
        var credentials = await Post<RefreshVm, AuthVm>($"auth/refresh", vm, true, cancellationToken);
        if (credentials is null)
        {
            await _credentials.Clear(cancellationToken);
            return;
        }

        await _credentials.Set(credentials, cancellationToken);
        await AttachOrRefreshTokenIfPossible(cancellationToken);
    }

    public string Curl(string endpoint, IFilter? filter)
    {
        var route = CreateEndpointUrl(endpoint, filter);
        return $"curl {_client.BaseAddress}{route}";
    }

    public async Task<HttpClient> Clone()
    {
        var client = new HttpClient();
        client.BaseAddress = _client.BaseAddress;

        await AttachOrRefreshTokenIfPossible(CancellationToken.None);
        if (_client.DefaultRequestHeaders.TryGetValues(ApiKeyHeaderName, out var apiKey))
            client.DefaultRequestHeaders.Add(ApiKeyHeaderName, apiKey.FirstOrDefault());

        if (_client.DefaultRequestHeaders.TryGetValues("Authorization", out var bearer))
            client.DefaultRequestHeaders.Add("Authorization", bearer.FirstOrDefault());

        return client;
    }

    private static string CreateEndpointUrl(string endpoint, IFilter? filter = null)
    {
        var route = $"{Prefix}/{endpoint}";

        var parameter = filter?.GetParameter()
            .Select(x => $"{x.Name}={x.Value}")
            .ToList() ?? new List<string>();

        if (parameter.Count <= 0)
            return route;

        var query = string.Join('&', parameter);
        route += $"?{query}";
        return route;
    }
}