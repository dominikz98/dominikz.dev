using Blazored.LocalStorage;
using dominikz.shared.Enums;
using dominikz.shared.ViewModels.Auth;

namespace dominikz.dev.Utils;

public class CredentialStorage
{
    private readonly ILocalStorageService _localStorage;
    private AuthVm? _credentials;
    private const string AuthTokenKey = "AuthToken";

    public CredentialStorage(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<string?> GetToken(bool disableExpirationValidation = false, CancellationToken cancellationToken = default)
    {
        var credentials = await TryGetFromLocalstorage(cancellationToken);
        if (credentials is null)
            return null;

        if (disableExpirationValidation == false
            && credentials.TokenExpiration < DateTime.UtcNow)
            return null;

        return credentials.Token;
    }

    public async Task<string?> GetRefreshToken(CancellationToken cancellationToken = default)
    {
        var credentials = await TryGetFromLocalstorage(cancellationToken);
        if (credentials is null)
            return null;

        if (credentials.RefreshTokenExpiration < DateTime.UtcNow)
            return null;

        return credentials.RefreshToken;
    }

    public async Task<bool> HasRight(PermissionFlags permission, CancellationToken cancellationToken = default)
    {
        var credentials = await TryGetFromLocalstorage(cancellationToken);
        if (credentials == null)
            return false;

        return credentials.Permissions.HasFlag(permission);
    }

    public async Task<bool> IsLoggedIn(CancellationToken cancellationToken = default)
         => await TryGetFromLocalstorage(cancellationToken) != null;
        
    public async Task Set(AuthVm credentials, CancellationToken cancellationToken = default)
    {
        _credentials = credentials;
        await _localStorage.SetItemAsync(AuthTokenKey, _credentials, cancellationToken);
    }

    public async Task Clear(CancellationToken cancellationToken = default)
    {
        _credentials = null;
        await _localStorage.RemoveItemAsync(AuthTokenKey, cancellationToken);
    }

    private async Task<AuthVm?> TryGetFromLocalstorage(CancellationToken cancellationToken)
    {
        _credentials ??= await _localStorage.GetItemAsync<AuthVm>(AuthTokenKey, cancellationToken);
        if (_credentials is null)
            return null;
        
        var isValid = _credentials.TokenExpiration > DateTime.UtcNow || _credentials.RefreshTokenExpiration > DateTime.UtcNow;
        if (isValid == false)
            return null;

        return _credentials;
    }
}