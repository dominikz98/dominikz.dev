using dominikz.Application.Utils;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Blog;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Application.Endpoints.Blog;

[Tags("blog")]
[Authorize(Policy = Policies.Blog)]
[Authorize(Policy = Policies.CreateOrUpdate)]
[Route("api/blog/draft")]
public class GetDraft : EndpointController
{
    private readonly IMediator _mediator;

    public GetDraft(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Execute(Guid id, CancellationToken cancellationToken)
    {
        var vm = await _mediator.Send(new GetDraftQuery(id), cancellationToken);
        if (vm is null)
            return NotFound();

        return Ok(vm);
    }
}

public record GetDraftQuery(Guid Id) : IRequest<EditArticleVm?>;

public class GetDraftQueryHandler : IRequestHandler<GetDraftQuery, EditArticleVm?>
{
    private readonly DatabaseContext _database;
    
    public GetDraftQueryHandler(DatabaseContext database)
    {
        _database = database;
    }

    public async Task<EditArticleVm?> Handle(GetDraftQuery request, CancellationToken cancellationToken)
        => await _database.From<Article>()
            .Where(x => x.Id == request.Id)
            .AsNoTracking()
            .MapToEditVm()
            .FirstOrDefaultAsync(cancellationToken);
    }