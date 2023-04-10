using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace dominikz.Application.Endpoints.Movies;

[Tags("movies")]
[Route("api/movies")]
public class StreamMovie : EndpointController
{
    private readonly IMediator _mediator;

    public StreamMovie(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}/stream")]
    public async Task<IActionResult> Execute(Guid id, [FromQuery] string token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized();

        var stream = await _mediator.Send(new StreamMovieQuery(id, token), cancellationToken);
        if (stream == null)
            return NotFound();

        return PhysicalFile(stream.FilePath, "video/mp4", $"{stream.Id}.mp4", true);
    }
}

public record StreamMovieQuery(Guid Id, string Token) : IRequest<StreamToken?>;

public class StreamMovieQueryHandler : IRequestHandler<StreamMovieQuery, StreamToken?>
{
    private readonly IMemoryCache _cache;

    public StreamMovieQueryHandler(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<StreamToken?> Handle(StreamMovieQuery request, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue<StreamToken>($"{request.Id}_TOKEN", out var stream) == false)
            return Task.FromResult((StreamToken?)null);

        if (stream!.Token != request.Token)
            return Task.FromResult((StreamToken?)null);

        return Task.FromResult((StreamToken?)stream);
    }
}