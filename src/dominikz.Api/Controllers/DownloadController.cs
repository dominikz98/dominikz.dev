using dominikz.Api.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace dominikz.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DownloadController : ControllerBase
{
    private readonly IMediator _mediator;

    public DownloadController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetImage([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var file = await _mediator.Send(new GetImageQuery(id), cancellationToken);
        if (file is null)
            return NotFound();

        return File(file, "image/jpg");
    }
}