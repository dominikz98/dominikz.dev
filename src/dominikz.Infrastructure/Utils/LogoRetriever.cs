using System.Net;

namespace dominikz.Infrastructure.Utils;

public static class LogoRetriever
{
    public static async Task<Stream?> GetLogoAsStream(string url, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;
        
        var response = await new HttpClient().GetAsync(url, cancellationToken);
        if (response.StatusCode != HttpStatusCode.OK)
            return null;

        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }

    public static async Task<Stream?> GetFaviconAsStream(string url, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;
        
        var faviconUrl = new Uri(url).GetLeftPart(UriPartial.Authority) + "/favicon.ico";
        return await GetLogoAsStream(faviconUrl, cancellationToken);
    }
}