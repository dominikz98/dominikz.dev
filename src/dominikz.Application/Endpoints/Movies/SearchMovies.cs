using dominikz.Application.Utils;
using dominikz.Domain.Enums;
using dominikz.Domain.Enums.Movies;
using dominikz.Domain.Extensions;
using dominikz.Domain.Filter;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Movies;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider;
using dominikz.Infrastructure.Provider.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Application.Endpoints.Movies;

[Tags("movies")]
[Route("api/movies")]
[ResponseCache(Duration = 3600, VaryByQueryKeys = new[] { "*" })]
public class SearchMovies : EndpointController
{
    private readonly IMediator _mediator;

    public SearchMovies(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] SearchMoviesQuery query, CancellationToken cancellationToken)
    {
        var vms = await _mediator.Send(query, cancellationToken);
        return Ok(vms);
    }

    [HttpGet("search/count")]
    public async Task<IActionResult> Count([FromQuery] CountMoviesQuery filter, CancellationToken cancellationToken)
    {
        var count = await _mediator.Send(filter, cancellationToken);
        return Ok(count);
    }
}

public class SearchMoviesQuery : MoviesFilter, IRequest<IReadOnlyCollection<MovieVm>>
{
}

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
        var includeDrafts = _credentials.HasPermission(PermissionFlags.Movies);
        var movies = await _database.From<Movie>()
            .AsNoTracking()
            .ApplyFilter(request, includeDrafts)
            .MapToVm()
            .ToListAsync(cancellationToken);

        // attach image url
        foreach (var movie in movies)
            if (movie.ImageUrl != string.Empty)
                movie.ImageUrl = _linkCreator.CreateImageUrl(movie.ImageUrl, ImageSizeEnum.Vertical);

        return movies;
    }
}

public class CountMoviesQuery : MoviesFilter, IRequest<int>
{
}

public class CountMoviesQueryHandler : IRequestHandler<CountMoviesQuery, int>
{
    private readonly DatabaseContext _context;
    private readonly CredentialsProvider _credentials;

    public CountMoviesQueryHandler(DatabaseContext context, CredentialsProvider credentials)
    {
        _context = context;
        _credentials = credentials;
    }

    public async Task<int> Handle(CountMoviesQuery request, CancellationToken cancellationToken)
    {
        var includeDrafts = _credentials.HasPermission(PermissionFlags.Movies | PermissionFlags.CreateOrUpdate);
        return await _context.From<Movie>()
            .ApplyFilter(request, includeDrafts)
            .CountAsync(cancellationToken);
    }
}

internal static class MovieQueryExtensions
{
    public static IQueryable<Movie> ApplyFilter(this IQueryable<Movie> query, MoviesFilter filter, bool includeDrafts)
    {
        if (includeDrafts == false)
            query = query.Where(x => x.PublishDate != null);

        if (filter.Genres is not null and not MovieGenresFlags.All)
            foreach (var genre in filter.Genres.Value.GetFlags())
                query = query.Where(x => x.Genres.HasFlag(genre));

        if (!string.IsNullOrWhiteSpace(filter.Text))
            query = query.Where(x => EF.Functions.Like(x.Title, $"%{filter.Text}%") || x.ImdbId == x.Title);

        query = query.OrderBy(x => x.Title);

        if (filter.Start != null)
            query = query.Skip(filter.Start.Value);

        if (filter.Count != null)
            query = query.Take(filter.Count.Value);

        return query;
    }
}