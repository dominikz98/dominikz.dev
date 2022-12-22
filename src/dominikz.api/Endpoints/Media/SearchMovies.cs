using dominikz.api.Mapper;
using dominikz.api.Models;
using dominikz.api.Provider;
using dominikz.api.Utils;
using dominikz.shared;
using dominikz.shared.Contracts;
using dominikz.shared.Filter;
using dominikz.shared.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Endpoints.Media;

[Tags("medias/movies")]
[ApiController]
[Route("api/medias/movies")]
public class SearchMovies : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchMovies(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Execute([FromQuery] MoviesFilter filter, CancellationToken cancellationToken)
    {
        var query = new SearchMoviesQuery()
        {
            Genres = filter.Genres,
            Text = filter.Text
        };

        var vms = await _mediator.Send(query, cancellationToken);
        return Ok(vms);
    }
}

public class SearchMoviesQuery : MoviesFilter, IRequest<IReadOnlyCollection<MovieVM>>
{
}

public class SearchMoviesQueryHandler : IRequestHandler<SearchMoviesQuery, IReadOnlyCollection<MovieVM>>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;

    public SearchMoviesQueryHandler(DatabaseContext database, ILinkCreator linkCreator)
    {
        _database = database;
        _linkCreator = linkCreator;
    }

    public async Task<IReadOnlyCollection<MovieVM>> Handle(SearchMoviesQuery request, CancellationToken cancellationToken)
    {
        var query = _database.From<Movie>();

        if (request.Genres is not null or MovieGenresFlags.ALL)
            foreach (var genre in request.Genres.Value.GetFlags())
                query = query.Where(x => x.Genres.HasFlag(genre));

        if (!string.IsNullOrWhiteSpace(request.Text))
            query = query.Where(x => EF.Functions.Like(x.Title, $"%{request.Text}%"));

        var movies = await query.MapToVm()
            .OrderBy(x => x.Title)
            .ToListAsync(cancellationToken);

        // attach image url
        foreach (var movie in movies)
            if (movie.Image is not null)
                movie.Image.Url = _linkCreator.CreateImageUrl(movie.Image.Id, ImageSizeEnum.Vertical)?.ToString() ?? string.Empty;

        return movies;
    }
}