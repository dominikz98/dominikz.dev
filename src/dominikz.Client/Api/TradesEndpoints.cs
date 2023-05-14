using dominikz.Domain.Filter;
using dominikz.Domain.ViewModels.Trading;

namespace dominikz.Client.Api;

public class TradesEndpoints
{
    private readonly ApiClient _client;
    private const string Endpoint = "trades";

    public TradesEndpoints(ApiClient client)
    {
        _client = client;
    }

    public async Task<EarningCallVm?> GetCallById(int id, CancellationToken cancellationToken = default)
        => await _client.GetSingle<EarningCallVm>($"{Endpoint}/earningscalls/{id}", cancellationToken);

    public async Task<IReadOnlyCollection<EarningCallListVm>> SearchCalls(EarningsCallsFilter? filter, CancellationToken cancellationToken = default)
        => await _client.Get<EarningCallListVm>($"{Endpoint}/earningscalls", filter, cancellationToken);
}