using dominikz.Application.Utils;
using dominikz.Domain.Enums;
using dominikz.Domain.Enums.Media;
using dominikz.Domain.Extensions;
using dominikz.Domain.Filter;
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
public class SearchMovies : EndpointController
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

public class SearchMoviesQuery : MoviesFilter, IRequest<IReadOnlyCollection<MovieVm>> { }

public class SearchMoviesQueryHandler : IRequestHandler<SearchMoviesQuery, IReadOnlyCollection<MovieVm>>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;
    private readonly CredentialsProvider _credentials;

    public SearchMoviesQueryHandler(DatabaseContext database, ILinkCreator linkCreator, CredentialsProvider credentials)
    {
        _database = database;
        _linkCreator = linkCreator;
        _credentials = credentials;
    }

    public async Task<IReadOnlyCollection<MovieVm>> Handle(SearchMoviesQuery request, CancellationToken cancellationToken)
    {
        var query = _database.From<Movie>();

        if (_credentials.HasPermission(PermissionFlags.Media) == false)
            query = query.Where(x => x.PublishDate != null);
        
        if (request.Genres is not null or MovieGenresFlags.All)
            foreach (var genre in request.Genres.Value.GetFlags())
                query = query.Where(x => x.Genres.HasFlag(genre));

        if (!string.IsNullOrWhiteSpace(request.Text))
            query = query.Where(x => EF.Functions.Like(x.Title, $"%{request.Text}%"));

        var movies = await query.MapToVm()
            .OrderBy(x => x.Title)
            .ToListAsync(cancellationToken);

        // attach image url
        foreach (var movie in movies)
            if (movie.ImageUrl != string.Empty)
                movie.ImageUrl = _linkCreator.CreateImageUrl(movie.ImageUrl, ImageSizeEnum.Vertical);

        return movies;
    }
}