using dominikz.Domain.Filter;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Songs;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Api.Endpoints.Songs;

[Tags("songs")]
[Route("api/songs")]
public class SearchSongs : EndpointController
{
    private readonly IMediator _mediator;

    public SearchSongs(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Execute([FromQuery] SongsFilter filter, CancellationToken cancellationToken)
    {
        var query = new SearchSongsQuery()
        {
            Text = filter.Text
        };
        var vms = await _mediator.Send(query, cancellationToken);
        return Ok(vms);
    }
}

public class SearchSongsQuery : SongsFilter, IRequest<IReadOnlyCollection<SongVm>>
{
}

public class SearchSongsQueryHandler : IRequestHandler<SearchSongsQuery, IReadOnlyCollection<SongVm>>
{
    private readonly DatabaseContext _database;

    public SearchSongsQueryHandler(DatabaseContext database)
    {
        _database = database;
    }

    public async Task<IReadOnlyCollection<SongVm>> Handle(SearchSongsQuery request, CancellationToken cancellationToken)
        => await _database.From<Song>()
                .Include(x => x.Segments)
                .Where(x => EF.Functions.Like(x.Name, $"%{request.Text}%"))
                .MapToVm()
                .ToListAsync(cancellationToken);
}