using dominikz.api.Mapper;
using dominikz.api.Models;
using dominikz.api.Provider;
using dominikz.api.Utils;
using dominikz.kernel.Contracts;
using dominikz.kernel.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Endpoints.Cookbook;

[Tags("cookbook")]
[ApiController]
[Route("api/cookbook")]
public class GetRecipe : ControllerBase
{
    private readonly IMediator _mediator;

    public GetRecipe(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("recipes/{id:guid}")]
    public async Task<IActionResult> Execute(Guid id, CancellationToken cancellationToken)
    {
        var vm = await _mediator.Send(new GetRecipeQuery(id), cancellationToken);
        if (vm is null)
            return NotFound();

        return Ok(vm);
    }
}

public class GetRecipeQuery : IRequest<RecipeDetailVM?>
{
    public Guid Id { get; set; }

    public GetRecipeQuery(Guid id)
    {
        Id = id;
    }
}

public class GetRecipeQueryHandler : IRequestHandler<GetRecipeQuery, RecipeDetailVM?>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;

    public GetRecipeQueryHandler(DatabaseContext database, ILinkCreator linkCreator)
    {
        _database = database;
        _linkCreator = linkCreator;
    }

    public async Task<RecipeDetailVM?> Handle(GetRecipeQuery request, CancellationToken cancellationToken)
    {
        var recipe = await _database.From<Recipe>()
            .Include(x => x.File)
            .Include(x => x.RecipesFoodsMappings)
            .ThenInclude(x => x.Food)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (recipe is null)
            return null;

        // map client side
        var vm = recipe.MapToDetailVM();
        vm.Image!.Url = _linkCreator.CreateImageUrl(vm.Image.Id, ImageSizeEnum.Horizonal)?.ToString() ?? string.Empty;
        return vm;
    }
}