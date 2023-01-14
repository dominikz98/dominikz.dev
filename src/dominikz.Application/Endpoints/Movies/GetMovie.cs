using dominikz.Application.Utils;
using dominikz.Domain.Constants;
using dominikz.Domain.Enums;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Media;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider;
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
            .Include(x => x.File)
            .Include(x => x.MoviesPersonsMappings)
            .ThenInclude(x => x.Person!.File)
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (movie is null)
            return null;

        var vm = movie.MapToDetailVm();

        // attach image urls
        vm.ImageUrl = _linkCreator.CreateImageUrl(movie.File!.Id.ToString(), ImageSizeEnum.Poster);
        vm.AuthorImageUrl = _linkCreator.CreateImageUrl(Persons.DominikZettlId.ToString(), ImageSizeEnum.Avatar);

        foreach (var directorVm in vm.Directors)
            directorVm.ImageUrl = _linkCreator.CreateImageUrl(directorVm.ImageUrl, ImageSizeEnum.Avatar);

        foreach (var writerVm in vm.Writers)
            writerVm.ImageUrl = _linkCreator.CreateImageUrl(writerVm.ImageUrl, ImageSizeEnum.Avatar);

        foreach (var starVm in vm.Stars)
            starVm.ImageUrl = _linkCreator.CreateImageUrl(starVm.ImageUrl, ImageSizeEnum.Avatar);

        return vm;
    }
}