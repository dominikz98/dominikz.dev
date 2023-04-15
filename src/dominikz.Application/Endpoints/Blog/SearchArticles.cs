using dominikz.Application.Utils;
using dominikz.Domain.Enums;
using dominikz.Domain.Enums.Blog;
using dominikz.Domain.Filter;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Blog;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider;
using dominikz.Infrastructure.Provider.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Application.Endpoints.Blog;

[Tags("blog")]
[Route("api/blog")]
[ResponseCache(Duration = 3600, VaryByQueryKeys = new[] { "*" })]
public class SearchArticles : EndpointController
{
    private readonly IMediator _mediator;

    public SearchArticles(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] SearchArticlesQuery query, CancellationToken cancellationToken)
    {
        var articles = await _mediator.Send(query, cancellationToken);
        return Ok(articles);
    }

    [HttpGet("search/count")]
    public async Task<IActionResult> Count([FromQuery] CountArticlesQuery query, CancellationToken cancellationToken)
    {
        var count = await _mediator.Send(query, cancellationToken);
        return Ok(count);
    }
}

public class SearchArticlesQuery : ArticleFilter, IRequest<IReadOnlyCollection<ArticleVm>>
{
}

public class SearchArticlesQueryHandler : IRequestHandler<SearchArticlesQuery, IReadOnlyCollection<ArticleVm>>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;
    private readonly CredentialsProvider _credentials;

    public SearchArticlesQueryHandler(DatabaseContext database,
        ILinkCreator linkCreator,
        CredentialsProvider credentials)
    {
        _database = database;
        _linkCreator = linkCreator;
        _credentials = credentials;
    }

    public async Task<IReadOnlyCollection<ArticleVm>> Handle(SearchArticlesQuery request, CancellationToken cancellationToken)
    {
        // load articles from sources
        var articles = new List<ArticleVm>();
        if (request.Source is null or ArticleSourceEnum.Dz)
        {
            var includeDrafts = _credentials.HasPermission(PermissionFlags.Blog | PermissionFlags.CreateOrUpdate);
            var dzArticles = await _database.From<Article>()
                .AsNoTracking()
                .ApplyFilter(request, includeDrafts)
                .MapToVm()
                .ToListAsync(cancellationToken);

            articles.AddRange(dzArticles);
        }

        if (request.Source is null or ArticleSourceEnum.Noobit or ArticleSourceEnum.Medlan)
        {
            var extArticles = await _database.From<ExtArticleShadow>()
                .AsNoTracking()
                .ApplyFilter(request)
                .MapToVm()
                .ToListAsync(cancellationToken);

            articles.AddRange(extArticles);
        }

        AttachMissingLinks(articles);
        SetFeaturedFlags(articles);

        articles = articles.OrderByDescending(x => x.PublishDate)
            .ThenByDescending(x => x.Featured)
            .ThenBy(x => x.Title)
            .ToList();

        return articles;
    }

    private void AttachMissingLinks(IReadOnlyCollection<ArticleVm> articles)
    {
        foreach (var article in articles)
        {
            if (article.ImageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase) == false
                && Guid.TryParse(article.ImageUrl, out _))
                article.ImageUrl = _linkCreator.CreateImageUrl(article.ImageUrl, ImageSizeEnum.Horizontal);
        }
    }

    private void SetFeaturedFlags(IReadOnlyCollection<ArticleVm> articles)
    {
        // feature first 2 articles of every source
        foreach (var feature in articles.Where(x => x.Source == ArticleSourceEnum.Dz).Take(2))
            feature.Featured = true;

        foreach (var feature in articles.Where(x => x.Source == ArticleSourceEnum.Noobit).Take(2))
            feature.Featured = true;

        foreach (var feature in articles.Where(x => x.Source == ArticleSourceEnum.Medlan).Take(2))
            feature.Featured = true;
    }
}

public class CountArticlesQuery : ArticleFilter, IRequest<int>
{
}

public class CountArticlesQueryHandler : IRequestHandler<CountArticlesQuery, int>
{
    private readonly DatabaseContext _database;
    private readonly CredentialsProvider _credentials;

    public CountArticlesQueryHandler(DatabaseContext database, CredentialsProvider credentials)
    {
        _database = database;
        _credentials = credentials;
    }

    public async Task<int> Handle(CountArticlesQuery request, CancellationToken cancellationToken)
    {
        // load articles from sources
        var count = 0;
        if (request.Source is null or ArticleSourceEnum.Dz)
        {
            var includeDrafts = _credentials.HasPermission(PermissionFlags.Blog | PermissionFlags.CreateOrUpdate);
            count += await _database.From<Article>().ApplyFilter(request, includeDrafts).CountAsync(cancellationToken);
        }

        if (request.Source is null or ArticleSourceEnum.Noobit or ArticleSourceEnum.Medlan)
            count += await _database.From<ExtArticleShadow>().ApplyFilter(request).CountAsync(cancellationToken);

        return count;
    }
}

internal static class ArticleQueryExtensions
{
    public static IQueryable<Article> ApplyFilter(this IQueryable<Article> query, ArticleFilter filter, bool includeDrafts)
    {
        if (filter.Category is not null)
            query = query.Where(x => x.Category == filter.Category);

        if (!string.IsNullOrWhiteSpace(filter.Text))
            query = query.Where(x => EF.Functions.Like(x.Title, $"%{filter.Text}%"));

        if (includeDrafts == false)
            query = query.Where(x => x.PublishDate != null);

        query = query.OrderByDescending(x => x.PublishDate)
            .ThenBy(x => x.Title);

        if (filter.Start != null)
            query = query.Skip(filter.Start.Value);

        if (filter.Count != null)
            query = query.Take(filter.Count.Value);

        return query;
    }

    public static IQueryable<ExtArticleShadow> ApplyFilter(this IQueryable<ExtArticleShadow> query, ArticleFilter filter)
    {
        if (filter.Source is not null)
            query = query.Where(x => x.Source == filter.Source);

        if (filter.Category is not null)
            query = query.Where(x => x.Category == filter.Category);

        if (!string.IsNullOrWhiteSpace(filter.Text))
            query = query.Where(x => EF.Functions.Like(x.Title, $"%{filter.Text}%"));

        query = query.OrderByDescending(x => x.Date)
            .ThenBy(x => x.Title);

        if (filter.Start != null)
            query = query.Skip(filter.Start.Value);

        if (filter.Count != null)
            query = query.Take(filter.Count.Value);

        return query;
    }
}