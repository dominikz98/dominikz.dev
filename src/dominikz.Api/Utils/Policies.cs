namespace dominikz.Api.Utils;

public static class Policies
{
    public const string RateLimit = "100TokenRateLimit";
    public const string CreateOrUpdate = "HasCreateOrUpdatePermission";
    public const string Movies = "HasMoviesPermission";
    public const string Blog = "HasBlogPermission";
    public const string Cookbook = "HasCookbookPermission";
    public const string Account = "HasAccountPermission";
    public const string Trades = "HasTradesPermission";
}