using dominikz.api.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dominikz.api.Endpoints.Blog;

[Tags("blog")]
[Authorize(Policy = Policies.Blog)]
[Authorize(Policy = Policies.CreateOrUpdate)]
[Route("api/blog")]
public class AddArticle : EndpointController
{
    private readonly IMediator _mediator;

    public AddArticle(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
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
