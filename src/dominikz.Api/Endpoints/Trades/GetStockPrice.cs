using dominikz.Domain.Enums.Trades;
using dominikz.Domain.Models;
using dominikz.Infrastructure.Clients.Finance;
using dominikz.Infrastructure.Provider.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Api.Endpoints.Trades;

[Tags("trades")]
[Route("api/trades/stockprice")]
public class GetStockPrice : EndpointController
{
    private readonly IMediator _mediator;

    public GetStockPrice(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{ecId:int}")]
    public async Task<IActionResult> Execute(int ecId, CancellationToken cancellationToken)
    {
        var vms = await _mediator.Send(new GetStockPriceRequest(ecId), cancellationToken);
        return Ok(vms);
    }
}

public record GetStockPriceRequest(int EarningCallId) : IRequest<FhCandle?>;

public class GetStockPriceRequestHandler : IRequestHandler<GetStockPriceRequest, FhCandle?>
{
    private readonly FinnhubClient _finnhub;
    private readonly DatabaseContext _database;

    public GetStockPriceRequestHandler(FinnhubClient finnhub, DatabaseContext database)
    {
        _finnhub = finnhub;
        _database = database;
    }
    
    public async Task<FhCandle?> Handle(GetStockPriceRequest request, CancellationToken cancellationToken)
    {
        var call = await _database.From<EarningCall>()
            .Where(x => x.Id == request.EarningCallId)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (call == null)
            return null;

        call.Sources |= InformationSource.Finnhub;
        _database.Update(call);
        await _database.SaveChangesAsync(cancellationToken);
        
        var timestamp = call.Date.ToDateTime(call.Release);
        return await _finnhub.GetCandlesByISIN(call.Symbol, timestamp, cancellationToken);
    }
}