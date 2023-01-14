using dominikz.Domain.Enums;
using dominikz.Infrastructure.Extensions;
using dominikz.Infrastructure.Utils;
using Microsoft.AspNetCore.Http;

namespace dominikz.Infrastructure.Provider;

public class CredentialsProvider
{
    private readonly PermissionFlags _permissions;

    public CredentialsProvider(IHttpContextAccessor contextAccessor)
    {
        if (contextAccessor.HttpContext?.User.TryGetFromClaim<PermissionFlags>(JwtHelper.ClaimPermissions, out var permissions) ?? false)
            _permissions = permissions;
        else
            _permissions = PermissionFlags.None;
    }

    public bool HasPermission(PermissionFlags required)
        => _permissions.HasFlag(required);
}