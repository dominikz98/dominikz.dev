using dominikz.api.Models;
using dominikz.api.Models.Options;
using dominikz.api.Provider;
using dominikz.api.Utils;
using dominikz.shared.ViewModels.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace dominikz.api.Endpoints.Auth;

[Tags("auth")]
[ApiController]
[Route("api/auth/register")]
public class Register : ControllerBase
{
    private readonly IMediator _mediator;

    public Register(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Execute([FromBody] RegisterVm request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new RegisterRequest(request.Username, request.Email, request.Password), cancellationToken);
        if (string.IsNullOrWhiteSpace(response.Info) == false)
            return BadRequest(response);

        return Ok(response);
    }
}

public class RegisterRequest : IRequest<AuthVm>
{
    public RegisterRequest(string username, string email, string password)
    {
        Username = username;
        Email = email;
        Password = password;
    }

    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class RegisterRequestHandler : IRequestHandler<RegisterRequest, AuthVm>
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

    public async Task<AuthVm> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        // get account from database
        var account = await _database.From<Account>()
            .Where(x => EF.Functions.Like(x.Username, request.Username)
                        || EF.Functions.Like(x.Email, request.Email))
            .FirstOrDefaultAsync(cancellationToken);

        if (account is not null)
            return new AuthVm() { Info = "Username or Email already in use!" };

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