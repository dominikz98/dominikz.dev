using dominikz.api.Mapper;
using dominikz.api.Models;
using dominikz.api.Provider;
using dominikz.api.Utils;
using dominikz.shared.Contracts;
using dominikz.shared.Filter;
using dominikz.shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Endpoints.Cookbook;

[Tags("cookbook")]
[ApiController]
[Route("api/cookbook")]
public class SearchRecipes : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchRecipes(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("recipes/search")]
    public async Task<IActionResult> Execute([FromQuery] RecipesFilter filter, CancellationToken cancellationToken)
    {
        var query = new SearchRecipesQuery()
        {
            FoodIds = filter.FoodIds,
            Text = filter.Text,
            Category = filter.Category
        };
        var vms = await _mediator.Send(query, cancellationToken);
        return Ok(vms);
    }
}

public class SearchRecipesQuery : RecipesFilter, IRequest<IReadOnlyCollection<RecipeVM>> { }

public class SearchRecipesQueryHandler : IRequestHandler<SearchRecipesQuery, IReadOnlyCollection<RecipeVM>>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;

    public SearchRecipesQueryHandler(DatabaseContext database, ILinkCreator linkCreator)
    {
        _database = database;
        _linkCreator = linkCreator;
    }

    public async Task<IReadOnlyCollection<RecipeVM>> Handle(SearchRecipesQuery request, CancellationToken cancellationToken)
    {
        var query = _database.From<Recipe>()
            .AsNoTracking();

        // apply filter
        if (request.Category != null && request.Category != RecipeCategoryFlags.ALL)
            query = query.Where(x => x.Categories.HasFlag(request.Category));

        if (!string.IsNullOrWhiteSpace(request.Text))
            query = query.Where(x => EF.Functions.Like(x.Title, $"%{request.Text}%"));

        if (request.FoodIds.Count > 0)
            query = query.Where(x => x.RecipesFoodsMappings.Any(y => request.FoodIds.Contains(y.FoodId)));

        var recipes = await query.MapToVm()
            .OrderBy(x => x.Title)
            .ToListAsync(cancellationToken);

        // attach image url
        foreach (var recipe in recipes)
            recipe.Image!.Url = _linkCreator.CreateImageUrl(recipe.Image.Id, ImageSizeEnum.Horizontal)?.ToString() ?? string.Empty;

        return recipes;
    }
}