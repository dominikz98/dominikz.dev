using dominikz.api.Mapper;
using dominikz.api.Models;
using dominikz.api.Provider;
using dominikz.api.Provider.Noobit;
using dominikz.api.Utils;
using dominikz.shared.Enums;
using dominikz.shared.Filter;
using dominikz.shared.ViewModels.Blog;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Endpoints.Blog;

[Tags("blog")]
[Route("api/blog")]
public class SearchArticles : EndpointController
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
            Category = filter.Category,
            Source = filter.Source
        };
        var vms = await _mediator.Send(query, cancellationToken);
        return Ok(vms);
    }
}

public class SearchArticlesQuery : ArticleFilter, IRequest<IReadOnlyCollection<ArticleVm>>
{
}

public class SearchArticlesQueryHandler : IRequestHandler<SearchArticlesQuery, IReadOnlyCollection<ArticleVm>>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;
    private readonly NoobitClient _noobitClient;
    private readonly MedlanClient _medlanClient;
    private readonly CredentialsProvider _credentials;

    public SearchArticlesQueryHandler(DatabaseContext database,
        ILinkCreator linkCreator,
        NoobitClient noobitClient,
        MedlanClient medlanClient,
        CredentialsProvider credentials)
    {
        _database = database;
        _linkCreator = linkCreator;
        _noobitClient = noobitClient;
        _medlanClient = medlanClient;
        _credentials = credentials;
    }

    public async Task<IReadOnlyCollection<ArticleVm>> Handle(SearchArticlesQuery request, CancellationToken cancellationToken)
    {
        // load articles from sources
        var articles = new List<ArticleVm>();
        if (request.Source is null or ArticleSourceEnum.Dz)
        {
            var dzArticles = await LoadFromDatabase(request, cancellationToken);
            articles.AddRange(dzArticles);
        }

        if (request.Source is null or ArticleSourceEnum.Noobit)
        {
            var noobitArticles = await _noobitClient.GetArticlesByCategory(request.Category, cancellationToken);
            articles.AddRange(noobitArticles);
        }

        if (request.Source is null or ArticleSourceEnum.Medlan)
        {
            var medlanArticles = await _medlanClient.GetArticlesByCategory(request.Category);
            articles.AddRange(medlanArticles);
        }

        articles = PostFilterAndOrderArticles(request, articles).ToList();
        AttachMissingLinks(articles);
        SetFeaturedFlags(articles);

        return articles;
    }

    private IReadOnlyCollection<ArticleVm> PostFilterAndOrderArticles(SearchArticlesQuery request, IReadOnlyCollection<ArticleVm> articles)
    {
        var clone = new List<ArticleVm>(articles);

        // filter
        if (!string.IsNullOrWhiteSpace(request.Text))
            clone = clone.Where(x => x.Title.Contains(request.Text, StringComparison.OrdinalIgnoreCase)
                                     || x.Author!.Name.Contains(request.Text, StringComparison.OrdinalIgnoreCase)
                                     || x.Category.ToString().Contains(request.Text, StringComparison.OrdinalIgnoreCase))
                .ToList();

        // order 
        return clone.OrderByDescending(x => x.PublishDate)
            .ThenByDescending(x => x.Featured)
            .ThenBy(x => x.Title)
            .ToList();
    }

    private void AttachMissingLinks(IReadOnlyCollection<ArticleVm> articles)
    {
        foreach (var article in articles)
        {
            if (article.Author != null
                && article.Author.ImageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase) == false
                && Guid.TryParse(article.Author.ImageUrl, out _))
                article.Author!.ImageUrl = _linkCreator.CreateImageUrl(article.Author.ImageUrl, ImageSizeEnum.Avatar);

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

    private async Task<IReadOnlyCollection<ArticleVm>> LoadFromDatabase(SearchArticlesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _database.From<Article>()
            .Include(x => x.Author!.File)
            .AsNoTracking();

        // pre filter
        if (_credentials.HasPermission(PermissionFlags.Blog) == false)
            query = query.Where(x => x.PublishDate != null);

        if (request.Category is not null)
            query = query.Where(x => x.Category == request.Category);

        if (!string.IsNullOrWhiteSpace(request.Text))
            query = query.Where(x => EF.Functions.Like(x.Title, $"%{request.Text}%")
                                     || EF.Functions.Like(x.Author!.Name, $"%{request.Text}%"));

        var articles = await query.OrderByDescending(x => x.PublishDate)
            .ThenBy(x => x.Title)
            .MapToVm()
            .ToListAsync(cancellationToken);

        foreach (var article in articles)
            article.Path = _linkCreator.CreateBlogUrl(article.Id);

        return articles;
    }
}