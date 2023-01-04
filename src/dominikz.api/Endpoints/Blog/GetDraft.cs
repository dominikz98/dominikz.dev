using dominikz.api.Mapper;
using dominikz.api.Models;
using dominikz.api.Provider;
using dominikz.api.Utils;
using dominikz.shared.Contracts;
using dominikz.shared.ViewModels.Blog;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Endpoints.Blog;

[Tags("blog")]
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
    private readonly ILinkCreator _linkCreator;

    public GetDraftQueryHandler(DatabaseContext database, ILinkCreator linkCreator)
    {
        _database = database;
        _linkCreator = linkCreator;
    }

    public async Task<EditArticleVm?> Handle(GetDraftQuery request, CancellationToken cancellationToken)
        => await _database.From<Article>()
            .Where(x => x.Id == request.Id)
            .AsNoTracking()
            .MapToEditVm()
            .FirstOrDefaultAsync(cancellationToken);
    }