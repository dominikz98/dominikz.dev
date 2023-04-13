using dominikz.Application.Utils;
using dominikz.Domain.Enums;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Movies;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Application.Endpoints.Movies;

[Tags("movies")]
[Route("api/movies")]
public class GetMovie : EndpointController
{
    private readonly IMediator _mediator;

    public GetMovie(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Execute(Guid id, CancellationToken cancellationToken)
    {
        var vm = await _mediator.Send(new GetMovieQuery(id), cancellationToken);
        if (vm is null)
            return NotFound();

        return Ok(vm);
    }
}

public record GetMovieQuery(Guid Id) : IRequest<MovieDetailVm?>;

public class GetMovieQueryHandler : IRequestHandler<GetMovieQuery, MovieDetailVm?>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;

    public GetMovieQueryHandler(DatabaseContext database, ILinkCreator linkCreator)
    {
        _database = database;
        _linkCreator = linkCreator;
    }

    public async Task<MovieDetailVm?> Handle(GetMovieQuery request, CancellationToken cancellationToken)
    {
        var movie = await _database.From<Movie>()
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (movie is null)
            return null;

        var vm = movie.MapToDetailVm();

        // attach image urls
        vm.ImageUrl = _linkCreator.CreateImageUrl(movie.Id.ToString(), ImageSizeEnum.Poster);
        return vm;
    }
}