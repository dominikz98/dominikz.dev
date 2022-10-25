using dominikz.api.Models;
using dominikz.api.Provider;
using dominikz.kernel.Contracts;
using dominikz.kernel.Filter;
using dominikz.kernel.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Endpoints.Movies;


[Tags("medias/movies")]
[ApiController]
[Route("api/medias/movies")]
public class CrawlImdb : ControllerBase
{
    private readonly IMediator _mediator;

    public CrawlImdb(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("imdb/{imdbid}")]
    public async Task<IActionResult> Execute(string imdbid, CancellationToken cancellationToken)
    {
        var vm = await _mediator.Send(new CrawlImdbQuery(imdbid), cancellationToken);
        if (vm is null)
            return NotFound();

        return Ok(vm);
    }
}

public class CrawlImdbQuery : MoviesFilter, IRequest<ImdbVM?>
{
    public CrawlImdbQuery(string imdbId)
    {
        ImdbId = imdbId;
    }

    public string ImdbId { get; }
}

public class CrawlImdbQueryHandler : IRequestHandler<CrawlImdbQuery, ImdbVM?>
{
    private readonly ImdbClient _client;
    private readonly DatabaseContext _database;

    public CrawlImdbQueryHandler(ImdbClient client, DatabaseContext database)
    {
        _client = client;
        _database = database;
    }

    public async Task<ImdbVM?> Handle(CrawlImdbQuery request, CancellationToken cancellationToken)
    {
        var vm = await _client.GetById(request.ImdbId, cancellationToken);
        if (vm is null)
            return null;

        await SyncMovieInfos(vm, cancellationToken);

        return vm;
    }

    private async Task SyncMovieInfos(ImdbVM vm, CancellationToken cancellationToken)
    {
        var movie = await _database.From<Movie>()
            .Where(x => x.ImdbId == vm.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (movie is null)
            return;

        // infos
        movie.Plot = vm.Plot;
        movie.Year = int.TryParse(vm.Year, out var year) ? year : default;
        movie.Runtime = TryParseRuntime(vm.Runtime, out var runtime) ? runtime : default;
        movie.Rating = (int)((vm.Rating?.Star ?? 0) * 10);
        movie.Genres = (MovieGenresFlags)vm.Genre.Select(x => MapGenreFromVM(x)).Sum(x => (int)x);
        _database.Update(movie);

        // stars
        await AddAndLinkPerson(movie.Id, vm, PersonCategoryFlags.Director, cancellationToken);
        await AddAndLinkPerson(movie.Id, vm, PersonCategoryFlags.Writer, cancellationToken);
        await AddAndLinkPerson(movie.Id, vm, PersonCategoryFlags.Star, cancellationToken);

        await _database.SaveChangesAsync(cancellationToken);
    }

    private MovieGenresFlags MapGenreFromVM(string genre)
    {
        if (genre.Equals("sci-fi", StringComparison.OrdinalIgnoreCase))
            return MovieGenresFlags.SciFi;

        return Enum.Parse<MovieGenresFlags>(genre);
    }

    private static bool TryParseRuntime(string runtime, out TimeSpan value)
    {
        value = default;
        if (string.IsNullOrWhiteSpace(runtime))
            return false;

        var parts = runtime.Split(" ");
        if (parts.Length != 2)
            return false;

        var hoursAsString = new string(parts[0].Where(char.IsDigit).ToArray());
        if (!int.TryParse(hoursAsString, out var hours))
            return false;

        var minutesAsString = new string(parts[1].Where(char.IsDigit).ToArray());
        if (!int.TryParse(minutesAsString, out var minutes))
            return false;

        value = new TimeSpan(hours, minutes, 0);
        return true;
    }

    private async Task AddAndLinkPerson(Guid movieId, ImdbVM vm, PersonCategoryFlags category, CancellationToken cancellationToken)
    {
        var query = category == PersonCategoryFlags.Star ? "stars" : category.ToString();
        var persons = vm.Top_credits.Where(x => x.Name.Equals(query, StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault()
            ?.Value ?? new List<string>();

        foreach (var person in persons)
        {
            var dbPerson = await _database.From<Person>()
                .Where(x => EF.Functions.Like(x.Name, $"%{person}%"))
                .FirstOrDefaultAsync(cancellationToken);

            // add if required
            dbPerson ??= (await _database.AddAsync(new Person()
            {
                Id = Guid.NewGuid(),
                Category = category,
                Name = person,
                FileId = Guid.Empty
            }, cancellationToken)).Entity;

            // update if required
            if (!dbPerson.Category.HasFlag(category))
            {
                dbPerson.Category = dbPerson.Category | category;
                _database.Update(dbPerson);
            }

            // link to movie
            await _database.AddAsync(new MoviesPersonsMapping()
            {
                MovieId = movieId,
                PersonId = dbPerson.Id,
                Category = category
            }, cancellationToken);
        }
    }
}