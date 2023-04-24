// using dominikz.Api.Attributes;
// using dominikz.Api.Background;
// using dominikz.Api.Utils;
// using dominikz.Domain.Models;
// using dominikz.Infrastructure.Clients.Finance;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
//
// namespace dominikz.Api.Endpoints.Admin;
//
// [Tags("admin")]
// [ApiKey(RequiresMasterKey = true)]
// [Authorize(Policy = Policies.Trades)]
// [Authorize(Policy = Policies.CreateOrUpdate)]
// [ApiController]
// [Route("api/admin/earningscallsrefresher")]
// public class CallEarningsCallsRefresher : ControllerBase
// {
//     private readonly IServiceProvider _serviceProvider;
//
//     public CallEarningsCallsRefresher(IServiceProvider serviceProvider)
//     {
//         _serviceProvider = serviceProvider;
//     }
//
//     [HttpPost]
//     public async Task<IActionResult> Execute(CancellationToken cancellationToken)
//     {
//         var finnhub = _serviceProvider.GetRequiredService<FinnhubClient>();
//         var calendar = await finnhub.GetTodayEarningsCalendar(cancellationToken);
//         var calls = calendar.EarningsCalendar.Where(x => x.EpsActual != null)
//             .Where(x => x.EpsEstimate != null)
//             .Where(x => x.RevenueActual != null)
//             .Where(x => x.RevenueEstimate != null)
//             .Where(x => x.EpsActual > x.EpsEstimate)
//             .Where(x => x.RevenueActual > x.RevenueEstimate)
//             .ToList();
//
//         var result = new List<FhEarningSuprise>();
//         foreach (var call in calls)
//         {
//             result.AddRange(await finnhub.GetEpsSuprises(call.Symbol, cancellationToken));
//         }
//         
//         // var earningsCallsRefresher = _serviceProvider.GetServices<ITimeTriggeredWorker>()
//         //     .First(x => x.GetType() == typeof(EarningsCallsRefresher));
//         //
//         // var fakeLog = new WorkerLog()
//         // {
//         //     Worker = nameof(EarningsCallsRefresher)
//         // };
//         // await earningsCallsRefresher.Execute(fakeLog, cancellationToken);
//         // return Ok(fakeLog);
//         return Ok(result);
//     }
// }
