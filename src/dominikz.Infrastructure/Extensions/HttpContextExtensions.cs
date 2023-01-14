using System.Security.Claims;

namespace dominikz.Infrastructure.Extensions;

public static class HttpContextExtensions
{
    public static bool TryGetFromClaim<T>(this ClaimsPrincipal user, string name, out T value) where T : struct, Enum
    {
        value = default;
        var claim = user.Claims.FirstOrDefault(x => x.Type.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (claim is null)
            return false;

        if (Enum.TryParse<T>(claim.Value, out var parsed) == false)
            return false;

        value = parsed;
        return true;
    }
}