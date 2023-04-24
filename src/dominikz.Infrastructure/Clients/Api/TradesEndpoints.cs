
namespace dominikz.Infrastructure.Clients.Api;

public class TradesEndpoints
{
    private readonly ApiClient _client;
    private const string Endpoint = "trades";

    public TradesEndpoints(ApiClient client)
    {
        _client = client;
    }
}