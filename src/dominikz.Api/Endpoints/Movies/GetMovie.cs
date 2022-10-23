using dominikz.api.Mapper;
using dominikz.api.Models;
using dominikz.api.Provider;
using dominikz.api.Utils;
using dominikz.kernel.Contracts;
using dominikz.kernel.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Endpoints.Movies;

[Tags("movies")]
[ApiController]
[Route("api/movies")]
public class GetMovie : ControllerBase
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

public class GetMovieQuery : IRequest<MovieDetailVM?>
{
    public Guid Id { get; set; }

    public GetMovieQuery(Guid id)
    {
        Id = id;
    }
}

public class GetMovieQueryHandler : IRequestHandler<GetMovieQuery, MovieDetailVM?>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;

    public GetMovieQueryHandler(DatabaseContext database, ILinkCreator linkCreator)
    {
        _database = database;
        _linkCreator = linkCreator;
    }

    public async Task<MovieDetailVM?> Handle(GetMovieQuery request, CancellationToken cancellationToken)
    {
        var movie = await _database.From<Movie>()
            .Include(x => x.File)
            .Include(x => x.Author!.File)
            .Include(x => x.MoviesPersonsMappings)
            .ThenInclude(x => x.Person!.File)
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (movie is null)
            return null;

        var vm = movie.MapToDetailVM();

        // attach image urls
        vm.Image!.Url = _linkCreator.CreateImageUrl(movie.File!.Id, ImageSizeEnum.Poster)?.ToString() ?? string.Empty;

        if (vm.Author?.Image is not null)
            vm.Author.Image.Url = _linkCreator.CreateImageUrl(vm.Author.Image.Id, ImageSizeEnum.Avatar)?.ToString() ?? string.Empty;

        foreach (var directorVM in vm.Directors)
            directorVM.Image!.Url = _linkCreator.CreateImageUrl(directorVM.Image!.Id, ImageSizeEnum.Avatar)?.ToString() ?? string.Empty;

        foreach (var writerVM in vm.Writers)
            writerVM.Image!.Url = _linkCreator.CreateImageUrl(writerVM.Image!.Id, ImageSizeEnum.Avatar)?.ToString() ?? string.Empty;

        foreach (var starVM in vm.Stars)
            starVM.Image!.Url = _linkCreator.CreateImageUrl(starVM.Image!.Id, ImageSizeEnum.Avatar)?.ToString() ?? string.Empty;

        return vm;
    }
}
