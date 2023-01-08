using System.Text.Json;
using dominikz.api.Models;
using dominikz.api.Models.Options;
using dominikz.api.Provider;
using dominikz.api.Provider.JustWatch;
using dominikz.api.Utils;
using dominikz.shared.Enums;
using dominikz.shared.ViewModels;
using dominikz.shared.ViewModels.Media;
using IMDbApiLib;
using IMDbApiLib.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace dominikz.api.Endpoints.Media;

[Tags("medias/movies")]
[Authorize(Policy = Policies.Media)]
[Authorize(Policy = Policies.CreateOrUpdate)]
[Route("api/medias/movies/template")]
public class GetMovieTemplate : EndpointController
{
    private readonly IMediator _mediator;

    public GetMovieTemplate(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{imdbId}")]
    [ResponseCache(Duration = 86400)]
    public async Task<IActionResult> Execute(string imdbId, CancellationToken cancellationToken)
    {
        var vm = await _mediator.Send(new GetMovieTemplateQuery(imdbId), cancellationToken);
        if (vm is null)
            return NotFound();

        return Ok(vm);
    }
}

public record GetMovieTemplateQuery(string ImdbId) : IRequest<MovieTemplateVm?>;

public class GetMovieTemplateHandler : IRequestHandler<GetMovieTemplateQuery, MovieTemplateVm?>
{
    private readonly IOptions<ImdbOptions> _options;
    private readonly JustWatchClient _jwClient;
    private readonly DatabaseContext _database;

    public GetMovieTemplateHandler(IOptions<ImdbOptions> options, JustWatchClient jwClient, DatabaseContext database)
    {
        _options = options;
        _jwClient = jwClient;
        _database = database;
    }

    public async Task<MovieTemplateVm?> Handle(GetMovieTemplateQuery request, CancellationToken cancellationToken)
    {
        var client = new ApiLib(_options.Value.ApiKey);
        var imdbData = await client.TitleAsync(request.ImdbId, Language.en, "Posters");
        // var imdbDataJson = await File.ReadAllTextAsync("/home/dominikzettl/Downloads/cDyYpo4r", cancellationToken);
        // var imdbData = JsonSerializer.Deserialize<TitleData>(imdbDataJson);

        if (string.IsNullOrWhiteSpace(imdbData?.Title))
            return null;

        // get poster urls
        var posterUrls = new List<string>();
        if (imdbData.Image != string.Empty)
            posterUrls.Add(imdbData.Image);
        
        posterUrls.AddRange(imdbData.Posters.Posters.Select(x => x.Link).Take(10));
        
        var template = new MovieTemplateVm()
        {
            ImdbId = imdbData.Id,
            Title = imdbData.Title,
            Runtime = int.TryParse(imdbData.RuntimeMins, out var runtime) ? TimeSpan.FromMinutes(runtime) : TimeSpan.Zero,
            Plot = imdbData.Plot,
            Year = int.TryParse(imdbData.Year, out var year) ? year : 0,
            Rating = double.TryParse(imdbData.IMDbRating, out var rating) ? (int)(rating * 10) : 0,
            GenreRecommendations = imdbData.GenreList.Select(x => x.Value).ToList(),
            PosterUrls = posterUrls
        };

        await AttachPersonsFromDatabase(template, imdbData, cancellationToken);

        var jwId = await _jwClient.SearchMovieByName(template.Title, cancellationToken);
        if (jwId == null)
            return template;

        var jwData = await _jwClient.GetMovieById(jwId.Value, cancellationToken);
        if (jwData is null)
            return template;

        template.JustWatchId = jwId.Value;
        template.YouTubeId = jwData.Clips.FirstOrDefault(x => x.Provider.Equals("youtube", StringComparison.OrdinalIgnoreCase))?.ExternalId ?? string.Empty;
        return template;
    }

    private async Task AttachPersonsFromDatabase(MovieTemplateVm template, TitleData imdbData, CancellationToken cancellationToken)
    {
        var personList = imdbData.DirectorList
            .Union(imdbData.WriterList)
            .Union(imdbData.StarList)
            .ToList();

        var nameList = personList.Select(x => x.Name.ToLower()).ToList();
        var dbData = await _database.From<Person>()
            .Where(x => nameList.Contains(x.Name.ToLower()))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        foreach (var person in personList)
        {
            var db = dbData.FirstOrDefault(x => x.Name.Equals(person.Name, StringComparison.OrdinalIgnoreCase));

            // Directors
            if (imdbData.DirectorList.Any(x => x.Id == person.Id))
                template.Directors.Add(db == null
                    ? new EditPersonVm() { Id = Guid.NewGuid(), Name = person.Name, Category = PersonCategoryFlags.Director }
                    : new EditPersonVm() { Id = db.Id, Tracked = true, Name = db.Name, Category = PersonCategoryFlags.Director });

            // Writers
            if (imdbData.WriterList.Any(x => x.Id == person.Id))
                template.Writers.Add(db == null
                    ? new EditPersonVm() { Id = Guid.NewGuid(), Name = person.Name, Category = PersonCategoryFlags.Writer }
                    : new EditPersonVm() { Id = db.Id, Tracked = true, Name = db.Name, Category = PersonCategoryFlags.Writer });

            // Stars
            if (imdbData.StarList.Any(x => x.Id == person.Id))
                template.Stars.Add(db == null
                    ? new EditPersonVm() { Id = Guid.NewGuid(), Name = person.Name, Category = PersonCategoryFlags.Star }
                    : new EditPersonVm() { Id = db.Id, Tracked = true, Name = db.Name, Category = PersonCategoryFlags.Star });
        }

        template.Directors = template.Directors.DistinctBy(x => x.Name).ToList();
        template.Writers = template.Writers.DistinctBy(x => x.Name).ToList();
        template.Stars = template.Stars.DistinctBy(x => x.Name).ToList();
    }
}