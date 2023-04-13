using dominikz.Application.Utils;
using dominikz.Domain.Options;
using dominikz.Domain.ViewModels.Media;
using IMDbApiLib;
using IMDbApiLib.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace dominikz.Application.Endpoints.Movies;

[Tags("movies")]
[Authorize(Policy = Policies.Media)]
[Authorize(Policy = Policies.CreateOrUpdate)]
[Route("api/movies/template")]
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
    
    public GetMovieTemplateHandler(IOptions<ImdbOptions> options)
    {
        _options = options;
    }

    public async Task<MovieTemplateVm?> Handle(GetMovieTemplateQuery request, CancellationToken cancellationToken)
    {
        var client = new ApiLib(_options.Value.ApiKey);
        var imdbData = await client.TitleAsync(request.ImdbId, Language.en, "Posters");

        if (string.IsNullOrWhiteSpace(imdbData?.Title))
            return null;

        // get poster urls
        var posterUrls = new List<string>();
        if (imdbData.Image != string.Empty)
            posterUrls.Add(imdbData.Image);

        posterUrls.AddRange(imdbData.Posters.Posters.Select(x => x.Link).Take(10));

        return new MovieTemplateVm()
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
    }
}