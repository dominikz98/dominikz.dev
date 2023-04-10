using dominikz.Application.Utils;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Cookbook;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider.Database;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Application.Endpoints.Cookbook;

[Tags("cookbook")]
[Route("api/cookbook/foods")]
[Authorize(Policy = Policies.Cookbook)]
[Authorize(Policy = Policies.CreateOrUpdate)]
public class GetFood : EndpointController
{
    private readonly IMediator _mediator;

    public GetFood(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Execute(Guid id, CancellationToken cancellationToken)
    {
        var vm = await _mediator.Send(new GetFoodDraftQuery(id), cancellationToken);
        if (vm == null)
            return NotFound();
        
        return Ok(vm);
    }
}

public record GetFoodDraftQuery(Guid Id) : IRequest<FoodVm?>;

public class GetFoodDraftQueryHandler : IRequestHandler<GetFoodDraftQuery, FoodVm?>
{
    private readonly DatabaseContext _database;

    public GetFoodDraftQueryHandler(DatabaseContext database)
    {
        _database = database;
    }

    public async Task<FoodVm?> Handle(GetFoodDraftQuery request, CancellationToken cancellationToken)
        => await _database.From<Food>()
            .Include(x => x.Snapshots)
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .MapToDetailVm()
            .FirstOrDefaultAsync(cancellationToken);
}