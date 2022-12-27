using dominikz.api.Models;
using dominikz.api.Models.Options;
using dominikz.api.Provider;
using dominikz.api.Utils;
using dominikz.shared.ViewModels;
using dominikz.shared.ViewModels.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace dominikz.api.Endpoints.Auth;

[Tags("auth")]
[ApiController]
[Route("api/auth/refresh")]
public class Refresh : ControllerBase
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
        if (string.IsNullOrWhiteSpace(response.Info) == false)
            return BadRequest(response);

        return Ok(response);
    }
}

public class RefreshRequest : IRequest<AuthVm>
{
    public RefreshRequest(string refreshToken, string expiredToken)
    {
        RefreshToken = refreshToken;
        ExpiredToken = expiredToken;
    }

    public string ExpiredToken { get; set; }
    public string RefreshToken { get; set; }
}

public class RefreshRequestHandler : IRequestHandler<RefreshRequest, AuthVm>
{
    private readonly DatabaseContext _database;
    private readonly PasswordHasher _hasher;
    private readonly IOptions<JwtOptions> _options;

    public RefreshRequestHandler(DatabaseContext database, PasswordHasher hasher, IOptions<JwtOptions> options)
    {
        _database = database;
        _hasher = hasher;
        _options = options;
    }

    public async Task<AuthVm> Handle(RefreshRequest request, CancellationToken cancellationToken)
    {
        // https://code-maze.com/using-refresh-tokens-in-asp-net-core-authentication/

        // parse principals
        var principal = JwtHelper.GetPrincipalFromExpiredToken(request.ExpiredToken, _options.Value);
        var username = principal?.Identity?.Name;
        if (username is null)
            return new AuthVm() { Info = "Invalid credentials!" };

        // get account from database
        var account = await _database.From<Account>()
            .Where(x => EF.Functions.Like(x.Username, username))
            .FirstOrDefaultAsync(cancellationToken);

        if (account is null
            || account.RefreshExpiration < DateTime.UtcNow
            || account.RefreshToken != request.RefreshToken)
            return new AuthVm() { Info = "Invalid credentials!" };

        // update last login
        var refreshToken = JwtHelper.CreateRefreshToken(_options.Value);
        account.LastLogin = DateTime.UtcNow;
        account.RefreshToken = refreshToken.Value;
        account.RefreshExpiration = refreshToken.Expiration;
        await _database.SaveChangesAsync(cancellationToken);

        // create token
        var token = JwtHelper.CreateToken(account, _options.Value);
        return new AuthVm()
        {
            Token = token.Value,
            TokenExpiration = token.Expiration,
            RefreshToken = refreshToken.Value,
            RefreshTokenExpiration = refreshToken.Expiration,
            Rights = account.Rights
        };
    }
}