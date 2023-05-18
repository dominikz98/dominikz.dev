// using dominikz.Domain.Enums;
// using dominikz.Domain.Filter;
// using dominikz.Domain.Models;
// using dominikz.Domain.ViewModels.Trading;
// using dominikz.Infrastructure.Extensions;
// using dominikz.Infrastructure.Mapper;
// using dominikz.Infrastructure.Provider.Database;
// using MediatR;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
//
// namespace dominikz.Api.Endpoints.Trades;
//
// [Tags("trades")]
// [Route("api/trades/earningscalls")]
// public class SearchEarningsCalls : EndpointController
// {
//     private readonly IMediator _mediator;
//
//     public SearchEarningsCalls(IMediator mediator)
//     {
//         _mediator = mediator;
//     }
//
//     [HttpGet]
//     public async Task<IActionResult> Search([FromQuery] SearchEarningsCallsQuery query, CancellationToken cancellationToken)
//     {
//         var vms = await _mediator.Send(query, cancellationToken);
//         return Ok(vms);
//     }
// }
//
// public class SearchEarningsCallsQuery : EarningsCallsFilter, IRequest<IReadOnlyCollection<EarningCallListVm>>
// {
// }
//
// public class SearchEarningsCallsQueryHandler : IRequestHandler<SearchEarningsCallsQuery, IReadOnlyCollection<EarningCallListVm>>
// {
//     private readonly DatabaseContext _database;
//
//     public SearchEarningsCallsQueryHandler(DatabaseContext database)
//     {
//         _database = database;
//     }
//
//     public async Task<IReadOnlyCollection<EarningCallListVm>> Handle(SearchEarningsCallsQuery request, CancellationToken cancellationToken)
//     {
//         var date = request.Date ?? DateOnly.FromDateTime(DateTime.UtcNow);
//         var (todayStart, todayEnd) = date.ToUnixRange();
//         var (yesterdayStart, yesterdayEnd) = date.AddDays(-1).ToUnixRange();
//         
//         return await _database.From<EarningCall>()
//             .AsNoTracking()
//             .Where(x => (x.Time == EarningCallTime.BMO && x.UtcTimestamp >= todayStart && x.UtcTimestamp <= todayEnd)
//                         || (x.Time == EarningCallTime.AMC && x.UtcTimestamp >= yesterdayStart && x.UtcTimestamp <= yesterdayEnd))
//             .MapToVm()
//             .ToListAsync(cancellationToken);
//     }
// }