using dominikz.Infrastructure.Clients;
using dominikz.Infrastructure.Clients.Noobit;

namespace dominikz.Application.Background;

public class CacheRefresher
{
    private readonly MedlanClient _medlanClient;
    private readonly NoobitClient _noobitClient;

    public CacheRefresher(MedlanClient medlanClient, NoobitClient noobitClient)
    {
        _medlanClient = medlanClient;
        _noobitClient = noobitClient;
    }
    
    public async Task Refresh(CancellationToken cancellationToken)
    {
        // Noobit/Medlan Articles
        await _medlanClient.GetArticlesByCategory(null, true);
        await _noobitClient.GetArticlesByCategory(null, true, cancellationToken);
    }
}