using dominikz.Application.Attributes;
using dominikz.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace dominikz.Application.Utils;

public interface ILinkCreator
{
    Uri? CreateRssUrl();
    string CreateBlogUrl(Guid articleId);
    string CreateImageUrl(string fileId, ImageSizeEnum size);
    string CreateLogoUrl(string symbol);
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
        var url = GetUri("~/blog/rss");
        if (string.IsNullOrWhiteSpace(url))
            return null;

        return new Uri(url);
    }

    public string CreateBlogUrl(Guid articleId)
        => GetUri($"~/blog/{articleId}");

    public string CreateImageUrl(string fileId, ImageSizeEnum size)
        => GetUri($"~/download/image/{fileId}/{(int)size}");

    public string CreateLogoUrl(string symbol)
    {
        var url = $"~/download/logo/{symbol}";
        if (_contextAccessor.HttpContext?.Request.Headers.TryGetValue(ApiKeyAttribute.ApiKeyHeaderName, out var apiKey) ?? false)
            url += $"?{ApiKeyAttribute.ApiKeyHeaderName}={apiKey}";

        return GetUri(url);
    }

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

        return string.IsNullOrWhiteSpace(url) ? string.Empty : url;
    }
}