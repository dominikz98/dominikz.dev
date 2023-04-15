using dominikz.Domain.Enums;
using dominikz.Domain.Options;
using dominikz.Infrastructure.Provider.Storage.Requests;
using HeyRed.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace dominikz.Application.Endpoints.Download;

[Tags("download")]
[Route("api/download/image")]
public class GetImage : EndpointController
{
    private readonly IOptions<ConnectionStrings> _options;

    public GetImage(IOptions<ConnectionStrings> options)
    {
        _options = options;
    }

    [HttpGet("{id:guid}/{size}")]
    public IActionResult Execute(Guid id, ImageSizeEnum size)
    {
        var filename = new DownloadImageRequest(id, size).Name;
        var path = Path.Combine(_options.Value.StorageProvider, filename);
        if (System.IO.File.Exists(path) == false)
            return NotFound();
        
        return PhysicalFile(path, MimeTypesMap.GetMimeType(Path.GetFileName(filename)));
    }
}