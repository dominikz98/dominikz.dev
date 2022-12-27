using System.Web;

namespace dominikz.dev.Utils;

public static class QueryUtils
{
    public static string AttachParam(string url, string name, string value)
    {
        var builder = new UriBuilder(url);
        var query = HttpUtility.ParseQueryString(builder.Query);
        query.Add(name, value);
        builder.Query = query.ToString();
        return builder.ToString();
    }
}