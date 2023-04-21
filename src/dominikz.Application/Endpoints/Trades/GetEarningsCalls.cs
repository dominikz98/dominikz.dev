using dominikz.Application.Utils;
using dominikz.Domain.Enums.Trades;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Trading;
using dominikz.Infrastructure.Clients.Finance;
using dominikz.Infrastructure.Mapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dominikz.Application.Endpoints.Trades;

[Tags("trades")]
[Authorize(Policy = Policies.Movies)]
[Authorize(Policy = Policies.CreateOrUpdate)]
[Route("api/trades/earningscalls")]
public class GetEarningsCalls : EndpointController
{
    private readonly IMediator _mediator;

    public GetEarningsCalls(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Execute(CancellationToken cancellationToken)
    {
        var vms = await _mediator.Send(new GetEarningsCallsRequest(), cancellationToken);
        return Ok(vms);
    }
}

public record GetEarningsCallsRequest : IRequest<IReadOnlyCollection<EarningCallVm>>;

public class GetEarningsCallsRequestHandler : IRequestHandler<GetEarningsCallsRequest, IReadOnlyCollection<EarningCallVm>>
{
    private readonly EarningsWhispersClient _eaClient;
    private readonly OnVistaClient _onVistaClient;
    private readonly AktienFinderClient _aktienFinderClient;
    private readonly FinanzenNetClient _finanzenNetClient;

    public GetEarningsCallsRequestHandler(EarningsWhispersClient eaClient,
        OnVistaClient onVistaClient,
        AktienFinderClient aktienFinderClient,
        FinanzenNetClient finanzenNetClient)
    {
        _eaClient = eaClient;
        _onVistaClient = onVistaClient;
        _aktienFinderClient = aktienFinderClient;
        _finanzenNetClient = finanzenNetClient;
    }

    public async Task<IReadOnlyCollection<EarningCallVm>> Handle(GetEarningsCallsRequest request, CancellationToken cancellationToken)
    {
        var earningsCalls = await _eaClient.GetEarningsCallsOfToday();
        foreach (var call in earningsCalls)
        {
            // resolve stock by symbol and name
            var stock = await GetStockBySymbolAndName(call, cancellationToken);
            if (stock == null)
                continue;

            call.ISIN = stock.ISIN;
            call.OnVistaLink = stock.Urls.Website;
            call.OnVistaNewsLink = stock.Urls.News;

            var logoUrl = await _aktienFinderClient.GetLogoByISIN(call.ISIN);
            if (string.IsNullOrWhiteSpace(logoUrl))
                continue;

            call.AktienFinderLogoLink = logoUrl;
            call.Sources |= InformationSource.AktienFinder;
        }

        return earningsCalls.MapToVm();
    }

    private async Task<OvResult?> GetStockBySymbolAndName(EarningCall call, CancellationToken cancellationToken)
    {
        var stock = await _onVistaClient.GetStockBySymbolAndName(call.Symbol, call.Name, cancellationToken);
        if (stock != null)
        {
            call.Sources |= InformationSource.OnVista;
            return stock;
        }

        // use finanzen.net fallback to find isin
        var isin = await _finanzenNetClient.GetISINBySymbolAndKeyword(call.Symbol, call.Name);
        isin ??= await _finanzenNetClient.GetISINBySymbolAndKeyword(call.Symbol, call.Symbol);
        if (string.IsNullOrWhiteSpace(isin))
            return null;

        call.Sources |= InformationSource.FinanzenNet;
        stock = await _onVistaClient.GetStockByISIN(isin, cancellationToken);
        if (stock == null)
            return null;

        call.Sources |= InformationSource.OnVista;
        return stock;
    }
}