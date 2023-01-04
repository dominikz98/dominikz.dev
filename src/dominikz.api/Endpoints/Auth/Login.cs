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
[Route("api/auth/login")]
public class Login : EndpointController
{
    private readonly IMediator _mediator;

    public Login(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Execute([FromBody] LoginVm request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new LoginRequest(request.Username, request.Password), cancellationToken);
        if (response.IsValid == false)
            return BadRequest(response.ToErrorList());

        return Ok(response.ViewModel);
    }
}

public record LoginRequest(string Username, string Password) : IRequest<ActionWrapper<AuthVm>>;

public class LoginRequestHandler : IRequestHandler<LoginRequest, ActionWrapper<AuthVm>>
{
    private readonly DatabaseContext _database;
    private readonly PasswordHasher _hasher;
    private readonly IOptions<JwtOptions> _options;

    public LoginRequestHandler(DatabaseContext database, PasswordHasher hasher, IOptions<JwtOptions> options)
    {
        _database = database;
        _hasher = hasher;
        _options = options;
    }

    public async Task<ActionWrapper<AuthVm>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        // get account from database
        var account = await _database.From<Account>()
            .Where(x => EF.Functions.Like(x.Username, request.Username))
            .FirstOrDefaultAsync(cancellationToken);

        if (account is null)
            return new ActionWrapper<AuthVm>("Invalid credentials!");

        // validate password hash
        var success = _hasher.Validate(account.Password, request.Password);
        if (success == false)
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