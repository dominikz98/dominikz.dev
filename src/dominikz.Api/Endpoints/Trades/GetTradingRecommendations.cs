using System.Collections;
using System.Text;
using CsvHelper;
using dominikz.Domain.Filter;
using dominikz.Domain.Models;
using dominikz.Infrastructure.Clients.Finance;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> Execute(CancellationToken cancellationToken)
    {
        var csv = await _mediator.Send(new GetTradingRecommendationsRequest(), cancellationToken);
        return File(csv, "text/csv", $"trade_recommendations_{DateTime.Now:yyyy_MM_dd}.csv");
    }
}

public class GetTradingRecommendationsRequest : EarningsCallsFilter, IRequest<MemoryStream>
{
}

public class GetTradingRecommendationsRequestHandler : IRequestHandler<GetTradingRecommendationsRequest, MemoryStream>
{
    private readonly FinnhubClient _finnhub;
    private readonly OnVistaClient _onVista;

    public GetTradingRecommendationsRequestHandler(FinnhubClient finnhub, OnVistaClient onVista)
    {
        _finnhub = finnhub;
        _onVista = onVista;
    }

    public async Task<MemoryStream> Handle(GetTradingRecommendationsRequest request, CancellationToken cancellationToken)
    {
        var date = request.Date ?? DateOnly.FromDateTime(DateTime.Now);
        var positiveEarningCalls = (await _finnhub.GetEarningsCalendar(date, cancellationToken))
            .EarningsCalendar
            .Where(x => x.EpsActual != null)
            .Where(x => x.EpsEstimate != null)
            .Where(x => x.RevenueActual != null)
            .Where(x => x.RevenueEstimate != null)
            .Where(x => x.EpsActual > x.EpsEstimate)
            .Where(x => x.RevenueActual > x.RevenueEstimate)
            .OrderBy(x => x.Hour)
            .ToList();

        var recommended = new List<EarningCall>();
        foreach (var call in positiveEarningCalls)
        {
            var epsSurprises = (await _finnhub.GetEpsSurprises(call.Symbol, cancellationToken))
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Quarter)
                .ToList();

            var current = epsSurprises.FirstOrDefault();
            if (current == null)
                continue;

            if (current.Surprise <= 0)
                continue;

            if (current.SurprisePercent <= 0)
                continue;

            var peek = epsSurprises.Select(x => x.Actual).Max();
            if (current.Actual < peek)
                continue;

            recommended.Add(new EarningCall
            {
                SurprisePercent = current.SurprisePercent,
                Surprise = current.Surprise,
                Date = DateOnly.FromDateTime(call.Date),
                EpsActual = call.EpsActual,
                EpsEstimate = call.EpsEstimate,
                RevenueActual = call.RevenueActual,
                RevenueEstimate = call.RevenueEstimate,
                Hour = call.Hour,
                Symbol = call.Symbol,
                Name = (await _finnhub.GetCompany(call.Symbol, cancellationToken))?.Name ?? string.Empty,
                ISIN = (await _onVista.SearchStockBySymbol(call.Symbol, cancellationToken))?.ISIN,
                Updated = DateTime.Now
            });
        }

        var ms = new MemoryStream();
        var streamWriter = new StreamWriter(ms, Encoding.UTF8);
        var csvWriter = new CsvWriter(streamWriter, System.Globalization.CultureInfo.CurrentCulture);
        await csvWriter.WriteRecordsAsync((IEnumerable)recommended, cancellationToken);
        await csvWriter.FlushAsync();
        ms.Position = 0;
        return ms;
    }
}