using dominikz.Api.Commands;
using dominikz.kernel.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace dominikz.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogController : ControllerBase
{
    private readonly IMediator _mediator;

    public BlogController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var vm = (await _mediator.Send(new SearchArticlesQuery() { Id = id }, cancellationToken)).FirstOrDefault();
        if (vm is null)
            return NotFound();

        return Ok(vm);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] ArticleFilter query, CancellationToken cancellationToken)
    {
        var filter = new SearchArticlesQuery()
        {
            Category = query.Category,
            Text = query.Text
        };
        var vms = await _mediator.Send(filter, cancellationToken);
        return Ok(vms);
    }
}