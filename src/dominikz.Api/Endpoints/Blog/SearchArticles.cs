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
    private readonly NoobitClient _noobitClient;

    public SearchArticlesQueryHandler(DatabaseContext database, ILinkCreator linkCreator, NoobitClient noobitClient)
    {
        _database = database;
        _linkCreator = linkCreator;
        _noobitClient = noobitClient;
    }

    public async Task<IReadOnlyCollection<ArticleListVM>> Handle(SearchArticlesQuery request, CancellationToken cancellationToken)
    {
        var dzArticles = await GetFromDatabase(request, cancellationToken);
        var noobitArticles = await GetFromNoobitDev(request, cancellationToken);

        // feature first 2 articles of every author
        var featureDZArticles = dzArticles.Take(2);
        foreach (var feature in featureDZArticles)
            feature.Featured = true;

        var featureTHArticles = noobitArticles.Take(2);
        foreach (var feature in featureTHArticles)
            feature.Featured = true;

        return  dzArticles
            .Union(noobitArticles)
            .OrderByDescending(x => x.Timestamp)
            .ThenBy(x => x.Title)
            .ToList();
    }

    private async Task<IReadOnlyCollection<ArticleListVM>> GetFromDatabase(SearchArticlesQuery request, CancellationToken cancellationToken)
    {
        var query = _database.From<Article>()
           .Include(x => x.Author!.File)
           .AsNoTracking();

        // filter
        if (request.Category != ArticleCategoryEnum.ALL)
            query = query.Where(x => x.Category == request.Category);

        if (!string.IsNullOrWhiteSpace(request.Text))
            query = query.Where(x => EF.Functions.Like(x.Title, $"%{request.Text}%")
                    || x.Tags.Any(y => EF.Functions.Like(y, $"%{request.Text}%"))
                    || EF.Functions.Like(x.Author!.Name, $"%{request.Text}%"));

        var articles = await query.OrderByDescending(x => x.Timestamp)
            .ThenBy(x => x.Title)
            .MapToVM()
            .ToListAsync(cancellationToken);

        // attach path
        foreach (var article in articles)
            article.Path = $"~/blog/{article.Id}";

        // attach image urls
        foreach (var article in articles)
        {
            article.Author!.Image!.Url = _linkCreator.CreateImageUrl(article.Author.Image.Id, ImageSizeEnum.Avatar)?.ToString() ?? string.Empty;
            article.Image!.Url = _linkCreator.CreateImageUrl(article.Image.Id, ImageSizeEnum.Horizonal)?.ToString() ?? string.Empty;
        }

        return articles;
    }

    private async Task<IReadOnlyCollection<ArticleListVM>> GetFromNoobitDev(SearchArticlesQuery request, CancellationToken cancellationToken)
    {
        var articles = new List<NoobitArticleVM>();

        articles = request.Category switch
        {
            ArticleCategoryEnum.ALL => await _noobitClient.GetAllArticles(cancellationToken),
            ArticleCategoryEnum.Coding => await _noobitClient.GetCodingArticles(cancellationToken),
            ArticleCategoryEnum.Travel => await _noobitClient.GetTravelArticles(cancellationToken),
            ArticleCategoryEnum.Birds => await _noobitClient.GetBirdsArticles(cancellationToken),
            ArticleCategoryEnum.Thoughts => await _noobitClient.GetThoughtsArticles(cancellationToken),
            _ => new List<NoobitArticleVM>()
        };

        // filter
        if (!string.IsNullOrWhiteSpace(request.Text))
            articles = articles.Where(x => x.Title.Contains(request.Text, StringComparison.OrdinalIgnoreCase)
                                        || x.Topic.Name.Contains(request.Text, StringComparison.OrdinalIgnoreCase))
                                .ToList();

        // attach author image url
        var vms = articles.MapToVM()
            .OrderByDescending(x => x.Timestamp)
            .ThenBy(x => x.Title)
            .ToList();

        foreach (var vm in vms)
            vm.Author!.Image!.Url = _linkCreator.CreateImageUrl(ArticleMapper.TobiasHaimerlId, ImageSizeEnum.Avatar)?.ToString() ?? string.Empty;

        return vms;
    }
}