using dominikz.Domain.Options;
using dominikz.Infrastructure.Provider.Storage.Requests;
using HeyRed.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace dominikz.Api.Endpoints.Download;

[Tags("download")]
[Route("api/download/logo")]
public class GetLogo : EndpointController
{
    private readonly IOptions<ConnectionStrings> _options;

    public GetLogo(IOptions<ConnectionStrings> options)
    {
        _options = options;
    }

    [HttpGet("{symbol}")]
    public IActionResult Execute(string symbol)
    {
        var filename = new DownloadLogoRequest(symbol).Name;
        var path = Path.Combine(_options.Value.StorageProvider, filename);
        if (System.IO.File.Exists(path) == false)
            return NotFound();
        
        return PhysicalFile(path, MimeTypesMap.GetMimeType(Path.GetFileName(filename)));
    }
}