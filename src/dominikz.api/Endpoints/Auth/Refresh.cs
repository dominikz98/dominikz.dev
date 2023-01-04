using dominikz.api.Models;
using dominikz.api.Models.Options;
using dominikz.api.Models.ViewModels;
using dominikz.api.Provider;
using dominikz.api.Utils;
using dominikz.shared.ViewModels.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace dominikz.api.Endpoints.Auth;

[Tags("auth")]
[Route("api/auth/refresh")]
public class Refresh : EndpointController
{
    private readonly IMediator _mediator;

    public Refresh(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Execute([FromBody] RefreshVm request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new RefreshRequest(request.RefreshToken, request.ExpiredToken), cancellationToken);
        if (response.IsValid == false)
            return BadRequest(response.ToErrorList());

        return Ok(response.ViewModel);
    }
}

public record RefreshRequest(string ExpiredToken, string RefreshToken) : IRequest<ActionWrapper<AuthVm>>;

public class RefreshRequestHandler : IRequestHandler<RefreshRequest, ActionWrapper<AuthVm>>
{
    private readonly DatabaseContext _database;
    private readonly IOptions<JwtOptions> _options;

    public RefreshRequestHandler(DatabaseContext database, IOptions<JwtOptions> options)
    {
        _database = database;
        _options = options;
    }

    public async Task<ActionWrapper<AuthVm>> Handle(RefreshRequest request, CancellationToken cancellationToken)
    {
        // parse principals
        var principal = JwtHelper.GetPrincipalFromExpiredToken(request.ExpiredToken, _options.Value);
        var username = principal?.Identity?.Name;
        if (username is null)
            return new ActionWrapper<AuthVm>("Invalid credentials!");

        // get account from database
        var account = await _database.From<Account>()
            .Where(x => EF.Functions.Like(x.Username, username))
            .FirstOrDefaultAsync(cancellationToken);

        if (account is null
            || account.RefreshExpiration < DateTime.UtcNow
            || account.RefreshToken != request.RefreshToken)
            return new ActionWrapper<AuthVm>("Invalid credentials!");

        // update last login
        var refreshToken = JwtHelper.CreateRefreshToken(_options.Value);
        account.LastLogin = DateTime.UtcNow;
        account.RefreshToken = refreshToken.Value;
        account.RefreshExpiration = refreshToken.Expiration;
        await _database.SaveChangesAsync(cancellationToken);

        // create token
        var token = JwtHelper.CreateToken(account, _options.Value);
        return new ActionWrapper<AuthVm>(new AuthVm()
        {
            Token = token.Value,
            TokenExpiration = token.Expiration,
            RefreshToken = refreshToken.Value,
            RefreshTokenExpiration = refreshToken.Expiration,
            Permissions = account.Permissions
        });
    }
}