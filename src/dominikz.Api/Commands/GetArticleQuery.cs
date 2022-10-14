using dominikz.api.Extensions;
using dominikz.api.Provider;
using dominikz.api.Utils;
using dominikz.kernel.Endpoints;
using dominikz.kernel.ViewModels;
using Markdig;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Commands;

public class GetArticleQuery : IRequest<ArticleDetailVM?>
{
    public Guid Id { get; set; }

    public GetArticleQuery(Guid id)
    {
        Id = id;
    }
}

public class GetArticleQueryHandler : IRequestHandler<GetArticleQuery, ArticleDetailVM?>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;

    public GetArticleQueryHandler(DatabaseContext database, ILinkCreator linkCreator)
    {
        _database = database;
        _linkCreator = linkCreator;
    }

    public async Task<ArticleDetailVM?> Handle(GetArticleQuery request, CancellationToken cancellationToken)
    {
        var filter = new ArticleFilter() { Id = request.Id };
        var article = await _database
            .Articles
            .Include(x => x.Author)
            .Search(filter)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (article is null)
            return null;

        // map viewmodels
        var vm = new ArticleDetailVM()
        {
            Id = article.Id,
            Author = article.Author!.Name,
            Category = article.Category,
            Timestamp = article.Timestamp,
            Title = article.Title,
            Tags = article.Tags,
            ImageUrl = _linkCreator.Create(article.FileId)?.ToString()
        };

        if (string.IsNullOrWhiteSpace(article.MDText))
            return vm;

        // convert markdown to html5
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        vm.HtmlText = Markdown.ToHtml(article.MDText!, pipeline);
        return vm;
    }
}
