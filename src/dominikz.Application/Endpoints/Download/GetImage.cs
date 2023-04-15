using dominikz.Application.ViewModels;
using dominikz.Domain.Enums;
using dominikz.Infrastructure.Provider.Storage;
using HeyRed.Mime;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace dominikz.Application.Endpoints.Download;

[Tags("download")]
[Route("api/download/image")]
public class GetImage : EndpointController
{
    private readonly IMediator _mediator;

    public GetImage(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("fresh/{id:guid}/{size}")]
    public async Task<IActionResult> ExecuteWithoutCache(Guid id, ImageSizeEnum size, CancellationToken cancellationToken)
        => await Execute(id, size, cancellationToken);

    [HttpGet("{id:guid}/{size}")]
    public async Task<IActionResult> Execute(Guid id, ImageSizeEnum size, CancellationToken cancellationToken)
    {
        var file = await _mediator.Send(new GetImageQuery(id, size), cancellationToken);
        if (file is null)
            return NotFound();

        return File(file.Data, file.ContentType, file.Name);
    }
}

public record GetImageQuery(Guid Id, ImageSizeEnum Size) : IRequest<FileDownloadWrapper?>;

public class GetImageQueryHandler : IRequestHandler<GetImageQuery, FileDownloadWrapper?>
{
    private readonly IStorageProvider _storage;

    public GetImageQueryHandler(IStorageProvider storage)
    {
        _storage = storage;
    }

    public async Task<FileDownloadWrapper?> Handle(GetImageQuery request, CancellationToken cancellationToken)
    {
        var fileRequest = new DownloadImageRequest(request.Id, request.Size);
        var file = await _storage.Download(fileRequest, cancellationToken);
        return file == null
            ? null
            : new FileDownloadWrapper(file, fileRequest.Name, MimeTypesMap.GetMimeType(fileRequest.Name));
    }
}