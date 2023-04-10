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
public class UpdateFood : EndpointController
{
    private readonly IMediator _mediator;

    public UpdateFood(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut]
    public async Task<IActionResult> Execute([FromBody] UpdateFoodRequest request, CancellationToken cancellationToken)
    {
        var vm = await _mediator.Send(request, cancellationToken);
        if (vm == null)
            return BadRequest(vm);

        return Ok(vm);
    }
}

public class UpdateFoodRequest : FoodVm, IRequest<FoodVm?>
{
}

public class UpdateFoodRequestHandler : IRequestHandler<UpdateFoodRequest, FoodVm?>
{
    private readonly DatabaseContext _database;

    public UpdateFoodRequestHandler(DatabaseContext database)
    {
        _database = database;
    }

    public async Task<FoodVm?> Handle(UpdateFoodRequest request, CancellationToken cancellationToken)
    {
        if (request.Id is null)
            return null;

        var food = await _database.From<Food>()
            .Include(x => x.Snapshots)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        
        if (food == null)
            return null;
        
        // update model
        food.ApplyChanges(request);
        _database.Update(food);

        await CreatePriceSnapshotIfRequired(food, request.Price, cancellationToken);
        await _database.SaveChangesAsync(cancellationToken);
        
        return food.MapToDetailVm();
    }

    private async Task CreatePriceSnapshotIfRequired(Food food, decimal price, CancellationToken cancellationToken)
    {
        if (price <= 0)
            return;

        var lastPrice = food.Snapshots
            .OrderBy(x => x.Timestamp)
            .Select(x => x.Price)
            .LastOrDefault();

        if (lastPrice == price)
            return;
        
        await _database.AddAsync(new FoodSnapshot()
        {
            Price = price,
            Timestamp = DateTime.Now,
            FoodId = food.Id
        }, cancellationToken);
    }
}