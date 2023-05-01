using dominikz.Domain.Enums;
using dominikz.Domain.ViewModels.Auth;

namespace dominikz.Client.Api;

public interface ICredentialStorage
{
    Task<bool> IsStreamingModeEnabled(CancellationToken cancellationToken = default);
    Task<string?> GetToken(bool disableExpirationValidation = false, CancellationToken cancellationToken = default);
    Task<string?> GetRefreshToken(CancellationToken cancellationToken = default);
    Task<bool> HasRight(PermissionFlags permission, CancellationToken cancellationToken = default);
    Task<bool> IsLoggedIn(CancellationToken cancellationToken = default);
    Task Set(AuthVm credentials, CancellationToken cancellationToken = default);
    Task Clear(CancellationToken cancellationToken = default);
}