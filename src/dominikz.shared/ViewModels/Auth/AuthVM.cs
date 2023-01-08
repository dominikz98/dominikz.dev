using System.Diagnostics.CodeAnalysis;
using dominikz.shared.Enums;

#pragma warning disable CS8618
namespace dominikz.shared.ViewModels.Auth;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class AuthVm
{
    public string Token { get; set; } = string.Empty;
    public DateTime TokenExpiration { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }
    public PermissionFlags Permissions { get; set; } = PermissionFlags.None;
    public string? Info { get; set; }
}