using Blazored.LocalStorage;
using dominikz.dev.Endpoints;
using dominikz.shared.Contracts;
using dominikz.shared.ViewModels.Auth;

namespace dominikz.dev.Utils;

public class AuthService
{
    public event Action? LoggedIn;
    public event Action? LoggedOut;
    
    private readonly AuthEndpoints _endpoints;
    private readonly ApiClient _client;
    private readonly ILocalStorageService _localStorage;
    private AuthVm? _auth;

    private const string AuthTokenKey = "AuthToken";

    public AuthService(AuthEndpoints endpoints, ApiClient client, ILocalStorageService localStorage)
    {
        _endpoints = endpoints;
        _client = client;
        _localStorage = localStorage;
    }

    public async Task<bool> HasRight(PermissionFlags permission, CancellationToken cancellationToken = default)
    {
        var isLoggedIn = await CheckIsLoggedIn(cancellationToken);
        if (isLoggedIn == false)
            return false;

        return _auth!.Permissions.HasFlag(permission);
    }

    public async Task<bool> CheckIsLoggedIn(CancellationToken cancellationToken = default)
    {
        _auth ??= await _localStorage.GetItemAsync<AuthVm>(AuthTokenKey, cancellationToken);
        if (_auth is null)
            return false;

        if (_auth.TokenExpiration < DateTime.UtcNow && _auth.RefreshTokenExpiration < DateTime.UtcNow)
            return false;

        return true;
    }

    public async Task Logout(CancellationToken cancellationToken = default)
    {
        // remove client token
        _client.RemoveAuthHeader();

        // remove stored data
        _auth = null;
        await _localStorage.RemoveItemAsync(AuthTokenKey, cancellationToken);
        
        LoggedOut?.Invoke();
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
        _auth = result;
        await _localStorage.SetItemAsync(AuthTokenKey, _auth, cancellationToken);

        LoggedIn?.Invoke();
        return true;
    }
}