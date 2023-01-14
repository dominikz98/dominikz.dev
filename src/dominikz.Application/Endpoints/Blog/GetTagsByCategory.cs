using dominikz.Application.Utils;
using dominikz.Domain.Enums;
using dominikz.Domain.Enums.Blog;
using dominikz.Domain.Models;
using dominikz.Infrastructure.Provider;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Application.Endpoints.Blog;

[Tags("blog")]
[Authorize(Policy = Policies.Blog)]
[Authorize(Policy = Policies.CreateOrUpdate)]
[Route("api/blog/tags")]
public class GetTagsByCategory : EndpointController
{
    private readonly IMediator _mediator;

    public GetTagsByCategory(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{category}")]
    public async Task<IActionResult> Execute([FromRoute] ArticleCategoryEnum category, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTagsByCategoryQuery(category), cancellationToken);
        return Ok(result);
    }
}

public record GetTagsByCategoryQuery(ArticleCategoryEnum Category) : IRequest<IReadOnlyCollection<string>>;

public class GetTagsByCategoryQueryHandler : IRequestHandler<GetTagsByCategoryQuery, IReadOnlyCollection<string>>
{
    private readonly DatabaseContext _database;

    public GetTagsByCategoryQueryHandler(DatabaseContext database)
        => _database = database;

    public async Task<IReadOnlyCollection<string>> Handle(GetTagsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var allTags = await _database.From<Article>()
            .Where(x => x.Category == request.Category)
            .Select(x => x.Tags)
            .ToListAsync(cancellationToken);

        return allTags.SelectMany(x => x)
            .Distinct()
            .OrderBy(x => x)
            .ToList();
    }
}