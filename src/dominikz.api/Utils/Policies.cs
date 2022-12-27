namespace dominikz.api.Utils;

public static class Policies
{
    public const string RateLimit = "100TokenRateLimit";
    public const string CreateOrUpdate = "HasCreateOrUpdatePermission";
    public const string Blog = "HasBlogPermission";
    public const string Media = "HasMediaPermission";
    public const string Account = "HasAccountPermission";
}