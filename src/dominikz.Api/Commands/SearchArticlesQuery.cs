using dominikz.Api.Extensions;
using dominikz.Api.Provider;
using dominikz.Api.Utils;
using dominikz.kernel.Endpoints;
using dominikz.kernel.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Api.Commands;

public class SearchArticlesQuery : ArticleFilter, IRequest<IReadOnlyCollection<ArticleListVM>> { }

public class SearchArticlesQueryHandler : IRequestHandler<SearchArticlesQuery, IReadOnlyCollection<ArticleListVM>>
{
    private readonly ILinkCreator _linkCreator;
    private readonly DatabaseContext _database;

    public SearchArticlesQueryHandler(ILinkCreator linkCreator, DatabaseContext database)
    {
        _linkCreator = linkCreator;
        _database = database;
    }

    public async Task<IReadOnlyCollection<ArticleListVM>> Handle(SearchArticlesQuery request, CancellationToken cancellationToken)
    {
        var articles = await _database
            .Articles
            .Include(x => x.Author)
            .Search(request)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // map viewmodels
        var vms = articles.Select(x => new ArticleListVM()
        {
            Id = x.Id,
            ImageUrl = _linkCreator.Create(x.FileId)!.ToString(),
            Author = x.Author!.Name,
            AuthorUrl = _linkCreator.Create(x.Author.FileId)!.ToString(),
            Category = x.Category,
            Timestamp = x.Timestamp,
            Title = x.Title,
            Available = !string.IsNullOrWhiteSpace(x.MDText)
        }).ToList();

        // feature latest 3 articles
        foreach (var vm in vms.Take(3))
            vm.Featured = true;

        return vms;
    }
}