using dominikz.Api.Utils;
using dominikz.Domain.Enums;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Movies;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider;
using dominikz.Infrastructure.Provider.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Api.Endpoints.Movies;

[Tags("movies")]
[Route("api/movies/preview")]
[ResponseCache(Duration = 86400)]
public class GetMoviePreview : EndpointController
{
    private readonly IMediator _mediator;

    public GetMoviePreview(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Execute(CancellationToken cancellationToken)
    {
        var vms = await _mediator.Send(new GetMoviePreviewQuery(), cancellationToken);
        return Ok(vms);
    }
}

public record GetMoviePreviewQuery : IRequest<IReadOnlyCollection<MoviePreviewVm>>;

public class GetMoviePreviewQueryHandler : IRequestHandler<GetMoviePreviewQuery, IReadOnlyCollection<MoviePreviewVm>>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;
    private readonly CredentialsProvider _credentials;

    public GetMoviePreviewQueryHandler(DatabaseContext database, ILinkCreator linkCreator, CredentialsProvider credentials)
    {
        _database = database;
        _linkCreator = linkCreator;
        _credentials = credentials;
    }

    public async Task<IReadOnlyCollection<MoviePreviewVm>> Handle(GetMoviePreviewQuery request, CancellationToken cancellationToken)
    {
        var previews = await _database.From<Movie>()
            .AsNoTracking()
            .Where(x => x.PublishDate != null)
            .OrderByDescending(x => x.PublishDate)
            .Take(6)
            .MapToPreviewVm()
            .ToListAsync(cancellationToken);

        foreach (var preview in previews)
            if (preview.ImageUrl != string.Empty)
                preview.ImageUrl = _linkCreator.CreateImageUrl(preview.ImageUrl, ImageSizeEnum.ThumbnailVertical);

        return previews;
    }
}