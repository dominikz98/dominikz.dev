using dominikz.Api.Utils;
using dominikz.Domain.Enums;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Blog;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Api.Endpoints.Blog;

[Tags("blog")]
[Route("api/blog")]
public class GetArticle : EndpointController
{
    private readonly IMediator _mediator;

    public GetArticle(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Execute(Guid id, CancellationToken cancellationToken)
    {
        var vm = await _mediator.Send(new GetArticleQuery(id), cancellationToken);
        if (vm is null)
            return NotFound();

        return Ok(vm);
    }
}

public record GetArticleQuery(Guid Id) : IRequest<ArticleViewVm?>;

public class GetArticleQueryHandler : IRequestHandler<GetArticleQuery, ArticleViewVm?>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;

    public GetArticleQueryHandler(DatabaseContext database, ILinkCreator linkCreator)
    {
        _database = database;
        _linkCreator = linkCreator;
    }

    public async Task<ArticleViewVm?> Handle(GetArticleQuery request, CancellationToken cancellationToken)
    {
        var article = await _database.From<Article>()
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .MapToViewVm()
            .FirstOrDefaultAsync(cancellationToken);

        if (article is null)
            return null;

        article.ImageUrl = _linkCreator.CreateImageUrl(article.ImageUrl, ImageSizeEnum.ThumbnailHorizontal);
        return article;
    }
}
