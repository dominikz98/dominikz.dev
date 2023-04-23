using dominikz.Api.Utils;
using dominikz.Domain.Enums;
using dominikz.Domain.Filter;
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
public class SearchRecipes : EndpointController
{
    private readonly IMediator _mediator;

    public SearchRecipes(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Execute([FromQuery] RecipeFilter filter, CancellationToken cancellationToken)
    {
        var vms = await _mediator.Send(new SearchRecipesQuery()
        {
            Text = filter.Text,
            FoodIds = filter.FoodIds,
            Flags = filter.Flags,
            Type = filter.Type
        }, cancellationToken);
        return Ok(vms);
    }
}

public class SearchRecipesQuery : RecipeFilter, IRequest<IReadOnlyCollection<RecipeListVm>>
{
}

public class SearchRecipesQueryHandler : IRequestHandler<SearchRecipesQuery, IReadOnlyCollection<RecipeListVm>>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;

    public SearchRecipesQueryHandler(DatabaseContext database, ILinkCreator linkCreator)
    {
        _database = database;
        _linkCreator = linkCreator;
    }

    public async Task<IReadOnlyCollection<RecipeListVm>> Handle(SearchRecipesQuery request, CancellationToken cancellationToken)
    {
        var query = _database.From<Recipe>()
            .Include(x => x.Ingredients)
            .ThenInclude(x => x.Food!.Snapshots)
            .AsQueryable();

        if (request.Type is not null)
            query = query.Where(x => x.Type == request.Type);

        if (request.Flags is not null)
            query = query.Where(x => x.Flags.HasFlag(request.Flags));

        if (!string.IsNullOrWhiteSpace(request.Text))
            query = query.Where(x => EF.Functions.Like(x.Name, $"%{request.Text}%"));

        if (request.FoodIds.Count > 0)
            query = query.Where(x => request.FoodIds.Any(y => x.Ingredients.Select(z => z.FoodId).Contains(y)));

        var recipes = await query
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);


        var vms = new List<RecipeListVm>();
        foreach (var recipe in recipes)
        {
            var vm = recipe.MapToListVm();
            
            // attach image url
            vm.ImageUrl = _linkCreator.CreateImageUrl(vm.Id.ToString(), ImageSizeEnum.ThumbnailVertical);
            vms.Add(vm);
        }

        return vms;
    }
}