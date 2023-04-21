using dominikz.Domain.ViewModels.Trading;

namespace dominikz.Infrastructure.Clients.Api;

public class TradesEndpoints
{
    private readonly ApiClient _client;
    private const string Endpoint = "trades";

    public TradesEndpoints(ApiClient client)
    {
        _client = client;
    }

    public async Task<TradeDetailVm?> GetById(int id, CancellationToken cancellationToken = default)
        => await _client.GetSingle<TradeDetailVm>($"{Endpoint}/{id}", cancellationToken);

    public async Task<TradeDetailVm?> Open(OpenTradeVm vm, CancellationToken cancellationToken = default)
        => await _client.Post<OpenTradeVm, TradeDetailVm>($"{Endpoint}", vm, false, cancellationToken);
}