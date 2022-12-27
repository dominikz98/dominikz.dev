namespace dominikz.api.Models.Options;

public class JwtOptions
{
    public bool ValidateIssuerSigningKey { get; set; }
    public string IssuerSigningKey { get; set; } = string.Empty;
    public bool ValidateIssuer { get; set; } = true;
    public string ValidIssuer { get; set; } = string.Empty;
    public bool ValidateAudience { get; set; } = true;
    public string ValidAudience { get; set; } = string.Empty;
    public bool RequireExpirationTime { get; set; }
    public int LifetimeInH { get; set; }
    public int RefreshTokenLength { get; set; }
    public int RefreshLifetimeInH { get; set; }
    public bool ValidateLifetime { get; set; } = true;
}