using System.Collections;
using System.Globalization;
using System.Text;
using CsvHelper;
using dominikz.Domain.Filter;
using dominikz.Domain.Models;
using dominikz.Infrastructure.Provider.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Api.Endpoints.Trades;

[Tags("trades")]
[Route("api/trades/recommendations")]
public class GetTradingRecommendations : EndpointController
{
    private readonly IMediator _mediator;

    public GetTradingRecommendations(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Execute([FromQuery] GetTradingRecommendationsRequest request, CancellationToken cancellationToken)
    {
        var csv = await _mediator.Send(request, cancellationToken);
        return File(csv, "text/csv", $"trade_recommendations_{DateTime.Now:yyyy_MM_dd}.csv");
    }
}

public class GetTradingRecommendationsRequest : EarningsCallsFilter, IRequest<MemoryStream>
{
}

public class GetTradingRecommendationsRequestHandler : IRequestHandler<GetTradingRecommendationsRequest, MemoryStream>
{
    private readonly DatabaseContext _context;

    public GetTradingRecommendationsRequestHandler(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<MemoryStream> Handle(GetTradingRecommendationsRequest request, CancellationToken cancellationToken)
    {
        var date = request.Date ?? DateOnly.FromDateTime(DateTime.Now);
        var shadows = await _context.From<FinnhubShadow>()
            .Where(x => x.Date == date)
            .AsNoTracking()
            .OrderBy(x => x.Date)
            .ThenBy(x => x.Hour)
            .ThenBy(x => x.Release)
            .ToListAsync(cancellationToken);

        var recommendations = shadows.Where(x => (x.ChartFlag ? 1 : 0) + (x.IncreaseFlag ? 1 : 0) + (x.PeakFlag ? 1 : 0) > 1).ToList();

        var ms = new MemoryStream();
        var streamWriter = new StreamWriter(ms, Encoding.UTF8);
        var csvWriter = new CsvWriter(streamWriter, CultureInfo.CurrentCulture);
        await csvWriter.WriteRecordsAsync((IEnumerable)recommendations, cancellationToken);
        await csvWriter.FlushAsync();
        ms.Position = 0;
        return ms;
    }
}