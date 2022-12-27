using System.Diagnostics.CodeAnalysis;
using dominikz.shared.Contracts;

namespace dominikz.api.Models;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class Account
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime LastLogin { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshExpiration { get; set; }
    public RightFlags Rights { get; set; }
}