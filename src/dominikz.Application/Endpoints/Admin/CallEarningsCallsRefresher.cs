using dominikz.Application.Attributes;
using dominikz.Application.Background;
using dominikz.Application.Utils;
using dominikz.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dominikz.Application.Endpoints.Admin;

[Tags("admin")]
[ApiKey(RequiresMasterKey = true)]
[Authorize(Policy = Policies.Trades)]
[Authorize(Policy = Policies.CreateOrUpdate)]
[ApiController]
[Route("api/admin/earningscallsrefresher")]
public class CallEarningsCallsRefresher : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public CallEarningsCallsRefresher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    [HttpPost]
    public async Task<IActionResult> Execute(CancellationToken cancellationToken)
    {
        var earningsCallsRefresher = _serviceProvider.GetServices<ITimeTriggeredWorker>()
            .First(x => x.GetType() == typeof(EarningsCallsRefresher));
        
        var fakeLog = new WorkerLog()
        {
            Worker = nameof(EarningsCallsRefresher)
        };
        await earningsCallsRefresher.Execute(fakeLog, cancellationToken);
        return Ok(fakeLog);
    }
}
