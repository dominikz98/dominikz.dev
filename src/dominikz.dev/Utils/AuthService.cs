using Blazored.LocalStorage;
using dominikz.dev.Endpoints;
using dominikz.shared.Contracts;
using dominikz.shared.ViewModels;
using dominikz.shared.ViewModels.Auth;

namespace dominikz.dev.Utils;

public class AuthService
{
    private readonly AuthEndpoints _endpoints;
    private readonly ApiClient _client;
    private readonly ILocalStorageService _localStorage;
    private RightFlags? _rights;

    private const string AuthTokenKey = "AuthToken";

    public AuthService(AuthEndpoints endpoints, ApiClient client, ILocalStorageService localStorage)
    {
        _endpoints = endpoints;
        _client = client;
        _localStorage = localStorage;
    }

    public bool HasRight(RightFlags right)
        => _rights?.HasFlag(right) ?? false;

    public async Task<bool> CheckIsLoggedIn(CancellationToken cancellationToken = default)
    {
        var authToken = await _localStorage.GetItemAsStringAsync(AuthTokenKey, cancellationToken);
        if (string.IsNullOrWhiteSpace(authToken))
            return false;

        _rights ??= await _endpoints.GetRights(cancellationToken);
        return true;
    }

    public async Task<bool> Login(LoginVm vm, CancellationToken cancellationToken = default)
    {
        // check for stored token
        var alreadyLoggedIn = await CheckIsLoggedIn(cancellationToken);
        if (alreadyLoggedIn)
            return true;
        
        // authenticate
        var result = await _endpoints.Login(vm, cancellationToken);
        if (string.IsNullOrWhiteSpace(result?.Token))
            return false;

        // set client token
        _client.SetAuthHeader(result.Token);

        // store data
        _rights = result.Rights;
        await _localStorage.SetItemAsStringAsync(AuthTokenKey, result.Token, cancellationToken);

        return true;
    }
}