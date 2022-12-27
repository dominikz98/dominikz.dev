using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dominikz.api.Endpoints.Blog;

[Tags("blog")]
[ApiController]
[Route("api/blog")]
public class AddArticle : ControllerBase
{
    private readonly IMediator _mediator;

    public AddArticle(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Execute(CancellationToken cancellationToken)
    {
        var vm = await _mediator.Send(new AddArticleRequest(), cancellationToken);
        return Ok(vm);
    }
}

public class AddArticleRequest : IRequest
{
}

public class AddArticleRequestHandler : IRequestHandler<AddArticleRequest, Unit>
{
    public Task<Unit> Handle(AddArticleRequest request, CancellationToken cancellationToken)
        => Unit.Task;
}
