using dominikz.api.Mapper;
using dominikz.api.Models;
using dominikz.api.Provider;
using dominikz.api.Utils;
using dominikz.kernel.Contracts;
using dominikz.kernel.Filter;
using dominikz.kernel.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Endpoints.Blog;

[Tags("blog")]
[ApiController]
[Route("api/blog")]
public class SearchArticles : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchArticles(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Execute([FromQuery] ArticleFilter filter, CancellationToken cancellationToken)
    {
        var query = new SearchArticlesQuery()
        {
            Text = filter.Text,
            Category = filter.Category
        };
        var vms = await _mediator.Send(query, cancellationToken);
        return Ok(vms);
    }
}

public class SearchArticlesQuery : ArticleFilter, IRequest<IReadOnlyCollection<ArticleListVM>> { }

public class SearchArticlesQueryHandler : IRequestHandler<SearchArticlesQuery, IReadOnlyCollection<ArticleListVM>>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;

    public SearchArticlesQueryHandler(DatabaseContext database, ILinkCreator linkCreator)
    {
        _database = database;
        _linkCreator = linkCreator;
    }

    public async Task<IReadOnlyCollection<ArticleListVM>> Handle(SearchArticlesQuery request, CancellationToken cancellationToken)
    {
        // apply filter
        var query = _database.From<Article>()
            .Include(x => x.Author!.File)
            .AsNoTracking();

        if (request.Category != ArticleCategoryEnum.ALL)
            query = query.Where(x => x.Category == request.Category);

        if (!string.IsNullOrWhiteSpace(request.Text))
            query = query.Where(x => EF.Functions.Like(x.Title, $"%{request.Text}%")
                    || x.Tags.Any(y => EF.Functions.Like(y, $"%{request.Text}%"))
                    || EF.Functions.Like(x.Author!.Name, $"%{request.Text}%"));

        var articles = await query
            .MapToVM()
            .OrderByDescending(x => x.Timestamp)
            .ThenBy(x => x.Title)
            .ToListAsync(cancellationToken);

        // feature latest 3 articles
        foreach (var article in articles.Take(3))
            article.Featured = true;

        // attach image url
        foreach (var article in articles)
        {
            article.Author!.Image!.Url = _linkCreator.CreateImageUrl(article.Author.Image.Id, ImageSizeEnum.Avatar)?.ToString() ?? string.Empty;
            article.Image!.Url = _linkCreator.CreateImageUrl(article.Image.Id, ImageSizeEnum.Horizonal)?.ToString() ?? string.Empty;
        }

        return articles;
    }
}