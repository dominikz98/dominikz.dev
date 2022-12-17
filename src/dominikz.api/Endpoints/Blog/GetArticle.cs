using dominikz.api.Mapper;
using dominikz.api.Models;
using dominikz.api.Provider;
using dominikz.api.Utils;
using dominikz.shared.Contracts;
using dominikz.shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Endpoints.Blog;

[Tags("blog")]
[ApiController]
[Route("api/blog")]
public class GetArticle : ControllerBase
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

public class GetArticleQuery : IRequest<ArticleDetailVm?>
{
    public Guid Id { get; set; }

    public GetArticleQuery(Guid id)
    {
        Id = id;
    }
}

public class GetArticleQueryHandler : IRequestHandler<GetArticleQuery, ArticleDetailVm?>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;

    public GetArticleQueryHandler(DatabaseContext database, ILinkCreator linkCreator)
    {
        _database = database;
        _linkCreator = linkCreator;
    }

    public async Task<ArticleDetailVm?> Handle(GetArticleQuery request, CancellationToken cancellationToken)
    {
        var article = await _database.From<Article>()
            .Include(x => x.Author!.File)
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .MapToDetailVm()
            .FirstOrDefaultAsync(cancellationToken);

        if (article is null)
            return null;

        article.Image!.Url = _linkCreator.CreateImageUrl(article.Image.Id, ImageSizeEnum.Horizontal)?.ToString() ?? string.Empty;
        article.Author!.Image!.Url = _linkCreator.CreateImageUrl(article.Author.Image.Id, ImageSizeEnum.Avatar)?.ToString() ?? string.Empty;

        article.Text = article.Text.ToHtml5();
        return article;
    }
}
