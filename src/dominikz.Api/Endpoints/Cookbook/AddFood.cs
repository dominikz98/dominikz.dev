using dominikz.Api.Utils;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Cookbook;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider.Database;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dominikz.Api.Endpoints.Cookbook;

[Tags("cookbook")]
[Route("api/cookbook/foods")]
[Authorize(Policy = Policies.Cookbook)]
[Authorize(Policy = Policies.CreateOrUpdate)]
public class AddFood : EndpointController
{
    private readonly IMediator _mediator;

    public AddFood(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Execute([FromBody] AddFoodRequest request, CancellationToken cancellationToken)
    {
        var vm = await _mediator.Send(request, cancellationToken);
        if (vm == null)
            return BadRequest(vm);

        return Ok(vm);
    }
}

public class AddFoodRequest : FoodVm, IRequest<FoodVm?>
{
}

public class AddFoodRequestHandler : IRequestHandler<AddFoodRequest, FoodVm?>
{
    private readonly DatabaseContext _database;

    public AddFoodRequestHandler(DatabaseContext database)
    {
        _database = database;
    }

    public async Task<FoodVm?> Handle(AddFoodRequest request, CancellationToken cancellationToken)
    {
        var toAdd = new Food().ApplyChanges(request);
        var food = await _database.AddAsync(toAdd, cancellationToken);

        // add price snapshot
        if (request.Price > 0)
            await _database.AddAsync(new FoodSnapshot()
            {
                Price = request.Price,
                Timestamp = DateTime.Now,
                FoodId = food.Entity.Id
            }, cancellationToken);

        await _database.SaveChangesAsync(cancellationToken);
        return food.Entity.MapToDetailVm();
    }
}