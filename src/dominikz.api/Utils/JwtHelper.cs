using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using dominikz.api.Models;
using dominikz.api.Models.Options;
using Microsoft.IdentityModel.Tokens;

namespace dominikz.api.Utils;

public static class JwtHelper
{
    public const string ClaimPermissions = "Permissions";
    
    public static JwtInfo CreateToken(Account account, JwtOptions options)
    {
        var key = Encoding.UTF8.GetBytes(options.IssuerSigningKey);
        var expiration = DateTime.UtcNow.AddHours(options.LifetimeInH);
        var claims = new Claim[]
        {
            new(ClaimTypes.Name, account.Username),
            new(ClaimTypes.Email, account.Email),
            new(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new(ClaimTypes.Expiration, expiration.ToString("yyyy-MM-dd HH:mm:ss tt")),
            new (ClaimPermissions, account.Permissions.ToString())
        };

        var token = new JwtSecurityToken(options.ValidIssuer,
            options.ValidAudience,
            claims,
            new DateTimeOffset(DateTime.UtcNow).DateTime,
            new DateTimeOffset(expiration).DateTime,
            new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));

        var tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
        return new JwtInfo(tokenAsString, expiration);
    }

    public static JwtInfo CreateRefreshToken(JwtOptions options)
    {
        var rnd = RandomNumberGenerator.GetBytes(options.RefreshTokenLength);
        return new JwtInfo(Convert.ToBase64String(rnd), DateTime.UtcNow.AddHours(options.RefreshLifetimeInH));
    }

    public static ClaimsPrincipal? GetPrincipalFromExpiredToken(string token, JwtOptions options)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = options.ValidateIssuerSigningKey,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.IssuerSigningKey)),
            ValidateIssuer = options.ValidateIssuer,
            ValidIssuer = options.ValidIssuer,
            ValidateAudience = options.ValidateAudience,
            ValidAudience = options.ValidAudience,
            RequireExpirationTime = options.RequireExpirationTime,
            ValidateLifetime = false
        };
        var principal = new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase) == false)
            return null;

        return principal;
    }
}

public record JwtInfo(string Value, DateTime Expiration);