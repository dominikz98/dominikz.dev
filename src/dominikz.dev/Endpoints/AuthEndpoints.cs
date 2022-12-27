using dominikz.shared.ViewModels.Auth;

namespace dominikz.dev.Endpoints;

public class AuthEndpoints
{
    private readonly ApiClient _client;
    private const string Endpoint = "auth";

    public AuthEndpoints(ApiClient client)
    {
        _client = client;
    }

    public async Task<AuthVm?> Login(LoginVm vm, CancellationToken cancellationToken)
        => await _client.Post<LoginVm, AuthVm>($"{Endpoint}/login", vm, true, cancellationToken);
}