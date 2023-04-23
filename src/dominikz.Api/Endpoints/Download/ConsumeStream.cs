using dominikz.Api.Utils;
using dominikz.Domain.Enums.Movies;
using Microsoft.AspNetCore.Mvc;

namespace dominikz.Api.Endpoints.Download;

[Tags("download")]
[Route("api/download/stream")]
public class ConsumeStream : EndpointController
{
    private readonly StreamTokenHandler _streamTokenHandler;

    public ConsumeStream(StreamTokenHandler streamTokenHandler)
    {
        _streamTokenHandler = streamTokenHandler;
    }

    [HttpGet("{prefix}/{id:guid}")]
    public IActionResult Execute(StreamTokenPrefix prefix, Guid id, [FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized();

        var stream = _streamTokenHandler.Verify(prefix, id, token);
        if (stream == null)
            return NotFound();

        return PhysicalFile(stream.FilePath, "video/mp4", $"{stream.Id}.mp4", true);
    }
}