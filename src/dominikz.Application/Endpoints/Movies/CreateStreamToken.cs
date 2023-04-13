using dominikz.Application.Utils;
using dominikz.Domain.Models;
using dominikz.Domain.Options;
using dominikz.Domain.ViewModels.Media;
using dominikz.Infrastructure.Provider.Database;
using dominikz.Infrastructure.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace dominikz.Application.Endpoints.Movies;

[Tags("movies")]
[Authorize(Policy = Policies.Media)]
[Route("api/movies")]
public class CreateStreamToken : EndpointController
{
    private readonly IMediator _mediator;

    public CreateStreamToken(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{id:guid}/stream/token")]
    public async Task<IActionResult> Execute(Guid id, CancellationToken cancellationToken)
    {
        var vm = await _mediator.Send(new CreateStreamTokenRequest(id), cancellationToken);
        if (vm == null)
            return NotFound();

        return Ok(vm);
    }
}

public record CreateStreamTokenRequest(Guid Id) : IRequest<StreamTokenVm?>;

public class CreateStreamTokenRequestHandler : IRequestHandler<CreateStreamTokenRequest, StreamTokenVm?>
{
    private readonly DatabaseContext _database;
    private readonly IOptions<ConnectionStrings> _connectionStrings;
    private readonly IMemoryCache _cache;
    private readonly PasswordHasher _hasher;

    public CreateStreamTokenRequestHandler(DatabaseContext database,
        IOptions<ConnectionStrings> connectionStrings,
        IMemoryCache cache,
        PasswordHasher hasher)
    {
        _database = database;
        _connectionStrings = connectionStrings;
        _cache = cache;
        _hasher = hasher;
    }

    public async Task<StreamTokenVm?> Handle(CreateStreamTokenRequest request, CancellationToken cancellationToken)
    {
        var stream = await _cache.GetOrCreateAsync($"{request.Id}_TOKEN", async (info) =>
        {
            info.AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(4);
            var fileName = await _database.From<Movie>()
                .Where(x => x.Id == request.Id)
                .Select(x => x.FilePath)
                .FirstOrDefaultAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(fileName))
                return null;

            var filePath = Path.Combine(_connectionStrings.Value.StorageProvider, "movies", fileName);
            var token = _hasher.GenerateHashedPassword();
            return new StreamToken(request.Id, Base64UrlEncoder.Encode(token), filePath, info.AbsoluteExpiration.Value);
        });

        if (stream == null)
            return null;

        return new StreamTokenVm()
        {
            Token = stream.Token,
            Expiration = stream.Expiration
        };
    }
}

public record StreamToken(Guid Id, string Token, string FilePath, DateTimeOffset Expiration);