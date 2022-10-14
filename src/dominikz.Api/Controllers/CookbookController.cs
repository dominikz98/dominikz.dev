using dominikz.api.Commands;
using dominikz.kernel.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace dominikz.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CookbookController : ControllerBase
{
    private readonly IMediator _mediator;

    public CookbookController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("foods")]
    public async Task<IActionResult> GetFoods(CancellationToken cancellationToken)
    {
        var vms = await _mediator.Send(new GetFoodsQuery(), cancellationToken);
        return Ok(vms);
    }

    [HttpGet("recipes/{id:guid}")]
    public async Task<IActionResult> Search(Guid id, CancellationToken cancellationToken)
    {
        var vm = await _mediator.Send(new GetRecipeQuery(id), cancellationToken);
        if (vm is null)
            return NotFound();

        return Ok(vm);
    }

    [HttpGet("recipes/search")]
    public async Task<IActionResult> Search([FromQuery] RecipesFilter query, CancellationToken cancellationToken)
    {
        var filter = new SearchRecipesQuery()
        {
            Category = query.Category,
            Text = query.Text,
            FoodIds = query.FoodIds
        };
        var vms = await _mediator.Send(filter, cancellationToken);
        return Ok(vms);
    }
}
