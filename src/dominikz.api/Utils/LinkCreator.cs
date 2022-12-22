using dominikz.shared.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace dominikz.api.Utils;

public interface ILinkCreator
{
    Uri? CreateRssUrl();
    Uri? CreateBlogUrl(Guid articleId);
    Uri? CreateImageUrl(Guid fileId, ImageSizeEnum size);
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
        => GetUri($"~/blog/rss");
    
    public Uri? CreateBlogUrl(Guid articleId)
        => GetUri($"~/blog/{articleId}");
    
    public Uri? CreateImageUrl(Guid fileId, ImageSizeEnum size)
        => GetUri($"~/download/image/{fileId}/{(int)size}");

    private Uri? GetUri(string relativePath)
    {
        var request = _contextAccessor.HttpContext?.Request;
        if (request is null)
            return null;

        var url = string.Concat(
            request.Scheme,
            "://",
            request.Host.ToUriComponent(),
            "/api",
            _urlHelper.Content(relativePath));

        if (string.IsNullOrWhiteSpace(url))
            return null;

        return new Uri(url);
    }
}
