using dominikz.Api.Commands;
using dominikz.kernel.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace dominikz.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediasController : ControllerBase
{
    private readonly IMediator _mediator;

    public MediasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var vm = (await _mediator.Send(new SearchMediasQuery() { Id = id }, cancellationToken)).FirstOrDefault();
        if (vm is null)
            return NotFound();

        return Ok(vm);
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
