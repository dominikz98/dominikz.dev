using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Cookbook;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Api.Endpoints.Cookbook;

[Tags("cookbook")]
[Route("api/cookbook/foods")]
public class GetFoods : EndpointController
{
    private readonly IMediator _mediator;

    public GetFoods(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Execute(CancellationToken cancellationToken)
    {
        var vms = await _mediator.Send(new GetFoodsQuery(), cancellationToken);
        return Ok(vms);
    }
}

public record GetFoodsQuery : IRequest<IReadOnlyCollection<FoodListVm>>;

public class GetFoodsQueryHandler : IRequestHandler<GetFoodsQuery, IReadOnlyCollection<FoodListVm>>
{
    private readonly DatabaseContext _database;

    public GetFoodsQueryHandler(DatabaseContext database)
    {
        _database = database;
    }

    public async Task<IReadOnlyCollection<FoodListVm>> Handle(GetFoodsQuery request, CancellationToken cancellationToken)
        => await _database.From<Food>()
            .AsNoTracking()
            .MapToVm()
            .ToListAsync(cancellationToken);
}