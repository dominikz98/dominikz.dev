using dominikz.Application.Attributes;
using dominikz.Application.Utils;
using dominikz.Application.ViewModels;
using dominikz.Domain.Models;
using dominikz.Domain.Options;
using dominikz.Domain.ViewModels.Auth;
using dominikz.Infrastructure.Provider.Database;
using dominikz.Infrastructure.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace dominikz.Application.Endpoints.Auth;

[Tags("auth")]
[ApiKey(RequiresMasterKey = true)]
[Authorize(Policy = Policies.Account)]
[Authorize(Policy = Policies.CreateOrUpdate)]
[ApiController]
[EnableRateLimiting(Policies.RateLimit)]
[Route("api/auth/register")]
public class Register : ControllerBase
{
    private readonly IMediator _mediator;

    public Register(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Execute([FromBody] RegisterVm request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new RegisterRequest(request.Username, request.Email, request.Password), cancellationToken);
        if (response.IsValid == false)
            return BadRequest(response.ToErrorList());

        return Ok(response.ViewModel);
    }
}

public record RegisterRequest(string Username, string Email, string Password) : IRequest<ActionWrapper<AuthVm>>;

public class RegisterRequestHandler : IRequestHandler<RegisterRequest, ActionWrapper<AuthVm>>
{
    private readonly DatabaseContext _database;
    private readonly PasswordHasher _hasher;
    private readonly IOptions<JwtOptions> _options;

    public RegisterRequestHandler(DatabaseContext database, PasswordHasher hasher, IOptions<JwtOptions> options)
    {
        _database = database;
        _hasher = hasher;
        _options = options;
    }

    public async Task<ActionWrapper<AuthVm>> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        // get account from database
        var account = await _database.From<Account>()
            .Where(x => EF.Functions.Like(x.Username, request.Username)
                        || EF.Functions.Like(x.Email, request.Email))
            .FirstOrDefaultAsync(cancellationToken);

        if (account is not null)
            return new ActionWrapper<AuthVm>("Username or Email already in use!");

        // hash  password
        var hash = _hasher.GenerateHash(request.Password);

        // create account
        var refreshToken = JwtHelper.CreateRefreshToken(_options.Value);
        account = new Account()
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Username = request.Username,
            Password = hash,
            RefreshToken = refreshToken.Value,
            RefreshExpiration = refreshToken.Expiration,
            LastLogin = DateTime.UtcNow
        };
        await _database.AddAsync(account, cancellationToken);
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