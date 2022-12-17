using dominikz.api.Mapper;
using dominikz.api.Models;
using dominikz.api.Provider;
using dominikz.shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Endpoints.Cookbook;

[Tags("cookbook")]
[ApiController]
[Route("api/cookbook")]
public class GetFoods : ControllerBase
{
    private readonly IMediator _mediator;

    public GetFoods(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("foods")]
    public async Task<IActionResult> Execute(CancellationToken cancellationToken)
    {
        var vms = await _mediator.Send(new GetFoodsQuery(), cancellationToken);
        return Ok(vms);
    }
}

public class GetFoodsQuery : IRequest<IReadOnlyCollection<FoodVM>> { }

public class GetFoodsQueryHandler : IRequestHandler<GetFoodsQuery, IReadOnlyCollection<FoodVM>>
{
    private readonly DatabaseContext _database;

    public GetFoodsQueryHandler(DatabaseContext database)
    {
        _database = database;
    }

    public async Task<IReadOnlyCollection<FoodVM>> Handle(GetFoodsQuery request, CancellationToken cancellationToken)
        => await _database.From<Food>()
            .AsNoTracking()
            .MapToVm()
            .OrderBy(x => x.Title)
            .ToListAsync(cancellationToken);
}
