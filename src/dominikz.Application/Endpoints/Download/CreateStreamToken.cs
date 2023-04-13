using System.Security.Claims;
using dominikz.Application.Utils;
using dominikz.Domain.Enums.Movies;
using dominikz.Domain.Models;
using dominikz.Domain.Options;
using dominikz.Domain.ViewModels.Movies;
using dominikz.Infrastructure.Provider.Database;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace dominikz.Application.Endpoints.Download;

[Tags("donwload")]
[Route("api/download/stream/token")]
public class CreateStreamToken : EndpointController
{
    private readonly IMediator _mediator;

    public CreateStreamToken(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Execute([FromBody] CreateStreamTokenVm request, CancellationToken cancellationToken)
    {
        var vm = await _mediator.Send(new CreateStreamTokenRequest()
        {
            Id = request.Id,
            Prefix = request.Prefix,
            Claims = User
        }, cancellationToken);

        if (vm == null)
            return NotFound();

        return Ok(vm);
    }
}

public class CreateStreamTokenRequest : CreateStreamTokenVm, IRequest<StreamTokenVm?>
{
    public ClaimsPrincipal? Claims { get; set; }
}

internal class CreateStreamTokenRequestHandler : IRequestHandler<CreateStreamTokenRequest, StreamTokenVm?>
{
    private readonly DatabaseContext _database;
    private readonly IOptions<ConnectionStrings> _connectionStrings;
    private readonly IAuthorizationService _authorizationHandler;
    private readonly StreamTokenHandler _tokenHandler;

    public CreateStreamTokenRequestHandler(DatabaseContext database,
        IOptions<ConnectionStrings> connectionStrings,
        IAuthorizationService authorizationHandler,
        StreamTokenHandler tokenHandler)
    {
        _database = database;
        _connectionStrings = connectionStrings;
        _authorizationHandler = authorizationHandler;
        _tokenHandler = tokenHandler;
    }

    public async Task<StreamTokenVm?> Handle(CreateStreamTokenRequest request, CancellationToken cancellationToken)
    {
        var movie = await _database.From<Movie>()
            .Where(x => x.Id == request.Id)
            .Select(x => new { x.FilePath, x.TrailerPath })
            .FirstOrDefaultAsync(cancellationToken);

        var movieAvailable = request.Claims != null && (await _authorizationHandler.AuthorizeAsync(request.Claims, Policies.Movies)).Succeeded;
        var filename = request.Prefix switch
        {
            StreamTokenPrefix.Movie => movieAvailable ? movie?.FilePath : null,
            StreamTokenPrefix.Trailer => movie?.TrailerPath,
            _ => null
        };

        if (string.IsNullOrWhiteSpace(filename))
            return null;

        var filepath = request.Prefix switch
        {
            StreamTokenPrefix.Movie => Path.Combine(_connectionStrings.Value.StorageProvider, "movies", movie!.FilePath!),
            StreamTokenPrefix.Trailer => Path.Combine(_connectionStrings.Value.StorageProvider, "trailer", movie!.TrailerPath!),
            _ => null
        };

        if (string.IsNullOrWhiteSpace(filepath))
            return null;

        return await _tokenHandler.Create(request.Prefix, request.Id, filepath, cancellationToken);
    }
}