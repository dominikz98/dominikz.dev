using dominikz.api.Mapper;
using dominikz.api.Models;
using dominikz.api.Provider;
using dominikz.api.Utils;
using dominikz.shared.Enums;
using dominikz.shared.ViewModels.Blog;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Endpoints.Blog;

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
            .Include(x => x.Author!.File)
            .Include(x => x.File)
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .MapToViewVm()
            .FirstOrDefaultAsync(cancellationToken);

        if (article is null)
            return null;

        article.ImageUrl = _linkCreator.CreateImageUrl(article.ImageUrl, ImageSizeEnum.Horizontal);
        article.Author!.ImageUrl = _linkCreator.CreateImageUrl(article.Author.ImageUrl, ImageSizeEnum.Avatar);
        return article;
    }
}
