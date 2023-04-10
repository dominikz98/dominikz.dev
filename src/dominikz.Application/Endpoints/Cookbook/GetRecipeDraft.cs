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
[Route("api/cookbook/recipes/draft")]
[Authorize(Policy = Policies.Cookbook)]
[Authorize(Policy = Policies.CreateOrUpdate)]
public class GetRecipeDraft : EndpointController
{
    private readonly IMediator _mediator;

    public GetRecipeDraft(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Execute(Guid id, CancellationToken cancellationToken)
    {
        var vm = await _mediator.Send(new GetRecipeDraftQuery(id), cancellationToken);
        if (vm == null)
            return NotFound();

        return Ok(vm);
    }
}

public record GetRecipeDraftQuery(Guid Id) : IRequest<RecipeVm?>;

public class GetRecipeDraftQueryHandler : IRequestHandler<GetRecipeDraftQuery, RecipeVm?>
{
    private readonly DatabaseContext _database;
    
    public GetRecipeDraftQueryHandler(DatabaseContext database)
    {
        _database = database;
    }

    public async Task<RecipeVm?> Handle(GetRecipeDraftQuery request, CancellationToken cancellationToken)
        => await _database.From<Recipe>()
            .AsNoTracking()
            .Include(x => x.Steps)
            .Include(x => x.Ingredients)
            .ThenInclude(x => x.Food)
            .Where(x => x.Id == request.Id)
            .MapToVm()
            .FirstOrDefaultAsync(cancellationToken);
}