using dominikz.api.Commands;
using dominikz.kernel.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace dominikz.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediasController : ControllerBase
{
    private readonly IMediator _mediator;

    public MediasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] MediaFilter query, CancellationToken cancellationToken)
    {
        var filter = new SearchMediasQuery()
        {
            Category = query.Category,
            Text = query.Text,
            Genre = query.Genre,
            Count = query.Count,
            Index = query.Index
        };
        var vms = await _mediator.Send(filter, cancellationToken);
        return Ok(vms);
    }
}
