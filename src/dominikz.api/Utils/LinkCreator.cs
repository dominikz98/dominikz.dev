using dominikz.shared.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace dominikz.api.Utils;

public interface ILinkCreator
{
    Uri? CreateRssUrl();
    string CreateBlogUrl(Guid articleId);
    string CreateImageUrl(string fileId, ImageSizeEnum size);
}

public class LinkCreator : ILinkCreator
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IUrlHelper _urlHelper;

    public LinkCreator(IHttpContextAccessor contextAccessor, IUrlHelper urlHelper)
    {
        _contextAccessor = contextAccessor;
        _urlHelper = urlHelper;
    }

    public Uri? CreateRssUrl()
    {
        var url = GetUri($"~/blog/rss");
        if (string.IsNullOrWhiteSpace(url))
            return null;

        return new(url);
    }

    public string CreateBlogUrl(Guid articleId)
        => GetUri($"~/blog/{articleId}");

    public string CreateImageUrl(string fileId, ImageSizeEnum size)
        => GetUri($"~/download/image/{fileId}/{(int)size}");

    private string GetUri(string relativePath)
    {
        var request = _contextAccessor.HttpContext?.Request;
        if (request is null)
            return string.Empty;

        var url = string.Concat(
            request.Scheme,
            "://",
            request.Host.ToUriComponent(),
            "/api",
            _urlHelper.Content(relativePath));

        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        return url;
    }
}