using dominikz.Api.Utils;
using dominikz.Domain.Enums;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Cookbook;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Api.Endpoints.Cookbook;

[Tags("cookbook")]
[Route("api/cookbook/recipes")]
public class GetRecipe : EndpointController
{
    private readonly IMediator _mediator;

    public GetRecipe(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Execute(Guid id, CancellationToken cancellationToken)
    {
        var vm = await _mediator.Send(new GetRecipeQuery(id), cancellationToken);
        if (vm == null)
            return NotFound();

        return Ok(vm);
    }
}

public record GetRecipeQuery(Guid Id) : IRequest<RecipeDetailVm?>;

public class GetRecipeQueryHandler : IRequestHandler<GetRecipeQuery, RecipeDetailVm?>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;

    public GetRecipeQueryHandler(DatabaseContext database, ILinkCreator linkCreator)
    {
        _database = database;
        _linkCreator = linkCreator;
    }

    public async Task<RecipeDetailVm?> Handle(GetRecipeQuery request, CancellationToken cancellationToken)
    {
        var vm = await _database.From<Recipe>()
            .AsNoTracking()
            .Include(x => x.Steps)
            .Include(x => x.Ingredients)
            .ThenInclude(x => x.Food)
            .Where(x => x.Id == request.Id)
            .MapToDetailVm()
            .FirstOrDefaultAsync(cancellationToken);

        if (vm == null)
            return null;

        // attach image url
        vm.ImageUrl = _linkCreator.CreateImageUrl(vm.Id.ToString(), ImageSizeEnum.ThumbnailHorizontal);

        // attach price snapshots
        var foodIds = vm.Ingredients.Select(x => x.Id).Distinct().ToList();
        var snapshotsByFood = await _database.From<FoodSnapshot>()
            .Where(x => foodIds.Contains(x.FoodId))
            .OrderByDescending(x => x.Timestamp)
            .GroupBy(x => new { x.FoodId, x.Timestamp.Date })
            .Select(x => new { x.Key.FoodId, x.Key.Date, Price = x.Average(y => (double)y.Price) })
            .Take(6)
            .ToListAsync(cancellationToken);

        var snapshots = new Dictionary<DateOnly, decimal>();
        foreach (var snapshot in snapshotsByFood)
        {
            var factor = vm.Ingredients.First(x => x.Id == snapshot.FoodId).Factor;
            var price = factor * Math.Round(factor * (decimal)snapshot.Price, 2, MidpointRounding.AwayFromZero);
            var date = DateOnly.FromDateTime(snapshot.Date);

            if (snapshots.ContainsKey(date))
                snapshots[date] += price;
            else
                snapshots[date] = price;
        }

        vm.PriceSnapshots = snapshots.OrderBy(x => x.Key)
            .Select(x => new PriceSnapshotVm()
            {
                Date = x.Key,
                Price = Math.Round(x.Value / vm.Portions,
                    2,
                    MidpointRounding.AwayFromZero)
            })
            .ToList();

        return vm;
    }
}