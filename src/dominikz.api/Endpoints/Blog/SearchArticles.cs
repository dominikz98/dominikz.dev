using dominikz.api.Mapper;
using dominikz.api.Models;
using dominikz.api.Provider;
using dominikz.api.Utils;
using dominikz.shared.Contracts;
using dominikz.shared.Filter;
using dominikz.shared.ViewModels;
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

public class SearchArticlesQuery : ArticleFilter, IRequest<IReadOnlyCollection<ArticleListVm>>  {  }

public class SearchArticlesQueryHandler : IRequestHandler<SearchArticlesQuery, IReadOnlyCollection<ArticleListVm>>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;
    private readonly NoobitClient _noobitClient;
    private readonly MedlanClient _medlanClient;

    public SearchArticlesQueryHandler(DatabaseContext database, ILinkCreator linkCreator, NoobitClient noobitClient, MedlanClient medlanClient)
    {
        _database = database;
        _linkCreator = linkCreator;
        _noobitClient = noobitClient;
        _medlanClient = medlanClient;
    }

    public async Task<IReadOnlyCollection<ArticleListVm>> Handle(SearchArticlesQuery request, CancellationToken cancellationToken)
    {
        // load articles from sources
        var dzArticles = await GetFromDatabase(request, cancellationToken);
        var noobitArticles = await _noobitClient.GetArticlesByCategory(request.Category, cancellationToken);
        var medlanArticles = _medlanClient.GetArticlesByCategory();

        var articles = dzArticles.Union(noobitArticles)
            .Union(medlanArticles)
            .ToList();

        articles = PostFilterAndOrderArticles(request, articles).ToList();
        AttachMissingLinks(articles);
        SetFeaturedFlags(articles);

        return articles;
    }

    private IReadOnlyCollection<ArticleListVm> PostFilterAndOrderArticles(SearchArticlesQuery request, IReadOnlyCollection<ArticleListVm> articles)
    {
        // filter
        if (!string.IsNullOrWhiteSpace(request.Text))
            articles = articles.Where(x => x.Title.Contains(request.Text, StringComparison.OrdinalIgnoreCase)
                    || x.Category.ToString().Contains(request.Text, StringComparison.OrdinalIgnoreCase))
                .ToList();

        // order 
        return articles.OrderByDescending(x => x.Timestamp)
            .ThenBy(x => x.Title)
            .ToList();
    }

    private void AttachMissingLinks(IReadOnlyCollection<ArticleListVm> articles)
    {
        foreach (var article in articles)
        {
            if (article.Author?.Image?.Url is not null and "")
                article.Author!.Image!.Url = _linkCreator.CreateImageUrl(article.Author.Id, ImageSizeEnum.Avatar)?.ToString() ?? string.Empty;
            
            if (article.Image?.Url is not null and "")
                article.Image!.Url = _linkCreator.CreateImageUrl(article.Image.Id, ImageSizeEnum.Horizontal)?.ToString() ?? string.Empty;
        }
            
    }

    private void SetFeaturedFlags(IReadOnlyCollection<ArticleListVm> articles)
    {
        // feature first 2 articles of every source
        foreach (var feature in articles.Where(x => x.Source == ArticleSource.DZ).Take(2))
            feature.Featured = true;
        
        foreach (var feature in articles.Where(x => x.Source == ArticleSource.Noobit).Take(2))
            feature.Featured = true;
        
        foreach (var feature in articles.Where(x => x.Source == ArticleSource.Medlan).Take(2))
            feature.Featured = true;
    }
    
    private async Task<IReadOnlyCollection<ArticleListVm>> GetFromDatabase(SearchArticlesQuery request, CancellationToken cancellationToken)
    {
        var query = _database.From<Article>()
            .Include(x => x.Author!.File)
            .AsNoTracking();

        // pre filter
        if (request.Category != ArticleCategoryEnum.ALL)
            query = query.Where(x => x.Category == request.Category);

        if (!string.IsNullOrWhiteSpace(request.Text))
            query = query.Where(x => EF.Functions.Like(x.Title, $"%{request.Text}%")
                                     || x.Tags.Any(y => EF.Functions.Like(y, $"%{request.Text}%"))
                                     || EF.Functions.Like(x.Author!.Name, $"%{request.Text}%"));

        return await query.OrderByDescending(x => x.Timestamp)
            .ThenBy(x => x.Title)
            .MapToVm()
            .ToListAsync(cancellationToken);
    }
}