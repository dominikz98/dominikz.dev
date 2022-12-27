using dominikz.api.Mapper;
using dominikz.api.Models;
using dominikz.api.Provider;
using dominikz.api.Utils;
using dominikz.shared;
using dominikz.shared.Contracts;
using dominikz.shared.Filter;
using dominikz.shared.ViewModels.Media;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Endpoints.Media;

[Tags("medias/books")]
[Route("api/medias/books")]
public class SearchBooks : EndpointController
{
    private readonly IMediator _mediator;

    public SearchBooks(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Execute([FromQuery] BooksFilter filter, CancellationToken cancellationToken)
    {
        var query = new SearchBooksQuery()
        {
            Text = filter.Text,
            Genres = filter.Genres,
            Language = filter.Language
        };

        var vms = await _mediator.Send(query, cancellationToken);
        return Ok(vms);
    }
}

public class SearchBooksQuery : BooksFilter, IRequest<IReadOnlyCollection<BookVM>> { }

public class SearchBooksQueryHandler : IRequestHandler<SearchBooksQuery, IReadOnlyCollection<BookVM>>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;

    public SearchBooksQueryHandler(DatabaseContext database, ILinkCreator linkCreator)
    {
        _database = database;
        _linkCreator = linkCreator;
    }

    public async Task<IReadOnlyCollection<BookVM>> Handle(SearchBooksQuery request, CancellationToken cancellationToken)
    {
        var query = _database.From<Book>();

        if (!string.IsNullOrWhiteSpace(request.Text))
            query = query.Where(x => EF.Functions.Like(x.Title, $"%{request.Text}%"));

        if (request.Genres is not null or BookGenresFlags.ALL)
            foreach (var genre in request.Genres.Value.GetFlags())
                query = query.Where(x => x.Genres.HasFlag(genre));

        if (request.Language is not null)
            query = query.Where(x => x.Language == request.Language);

        var books = await query.MapToVm()
            .OrderBy(x => x.Title)
            .ToListAsync(cancellationToken);

        // attach image url
        foreach (var book in books)
            if (book.Image is not null)
                book.Image.Url = _linkCreator.CreateImageUrl(book.Image.Id, ImageSizeEnum.Vertical)?.ToString() ?? string.Empty;

        return books;
    }
}