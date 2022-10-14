using Microsoft.AspNetCore.Mvc;

namespace dominikz.api.Utils;

public interface ILinkCreator
{
    Uri? Create(Guid fileId);
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

    public Uri? Create(Guid fileId)
        => GetUri($"~/download/{fileId}");

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
