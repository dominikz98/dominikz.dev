using dominikz.Api.Utils;
using dominikz.Domain.Enums;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Trading;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Api.Endpoints.Trades;

[Tags("trades")]
[Route("api/trades/earningscalls")]
public class GetEarningCall : EndpointController
{
    private readonly IMediator _mediator;

    public GetEarningCall(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Search(int id, CancellationToken cancellationToken)
    {
        var vm = await _mediator.Send(new GetEarningCallQuery(id), cancellationToken);
        if (vm == null)
            return NotFound();

        return Ok(vm);
    }
}

public record GetEarningCallQuery(int Id) : IRequest<EarningCallVm?>;

public class GetEarningCallQueryHandler : IRequestHandler<GetEarningCallQuery, EarningCallVm?>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;

    public GetEarningCallQueryHandler(DatabaseContext database, ILinkCreator linkCreator)
    {
        _database = database;
        _linkCreator = linkCreator;
    }

    public async Task<EarningCallVm?> Handle(GetEarningCallQuery request, CancellationToken cancellationToken)
    {
        var call = await _database.From<EarningCall>()
            .Where(x => x.Id == request.Id)
            .MapToDetailVm()
            .FirstOrDefaultAsync(cancellationToken);

        if (call == null)
            return null;

        if (string.IsNullOrWhiteSpace(call.LogoUrl) == false)
            call.LogoUrl = _linkCreator.CreateLogoUrl(call.Symbol);

        return call;
    }
}