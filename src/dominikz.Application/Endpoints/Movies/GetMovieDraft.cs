using dominikz.Application.Utils;
using dominikz.Domain.Enums;
using dominikz.Domain.Enums.Media;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Media;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Application.Endpoints.Movies;

[Tags("movies")]
[Authorize(Policy = Policies.Media)]
[Authorize(Policy = Policies.CreateOrUpdate)]
[Route("api/movies/draft")]
public class GetMovieDraft : EndpointController
{
    private readonly IMediator _mediator;

    public GetMovieDraft(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Execute(Guid id, CancellationToken cancellationToken)
    {
        var vm = await _mediator.Send(new GetMovieDraftQuery(id), cancellationToken);
        if (vm is null)
            return NotFound();

        return Ok(vm);
    }
}

public record GetMovieDraftQuery(Guid Id) : IRequest<EditMovieVm?>;

public class GetMovieDraftQueryHandler : IRequestHandler<GetMovieDraftQuery, EditMovieVm?>
{
    private readonly DatabaseContext _database;

    public GetMovieDraftQueryHandler(DatabaseContext database)
        => _database = database;

    public async Task<EditMovieVm?> Handle(GetMovieDraftQuery request, CancellationToken cancellationToken)
    {
        var movie = await _database.From<Movie>()
            .Where(x => x.Id == request.Id)
            .Include(x => x.MoviesPersonsMappings)
            .ThenInclude(x => x.Person)
            .AsNoTracking()
            .MapToEditVm()
            .FirstOrDefaultAsync(cancellationToken);

        if (movie == null)
            return null;

        movie.Genres = movie.Genres.Where(x => x != MovieGenresFlags.All).ToList();
        return movie;
    }
}