using dominikz.Application.Utils;
using dominikz.Domain.Enums.Trades;
using dominikz.Domain.Filter;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Trading;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Application.Endpoints.Trades;

[Tags("trades")]
[Route("api/trades/earningscalls")]
public class GetEarningsCalls : EndpointController
{
    private readonly IMediator _mediator;

    public GetEarningsCalls(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Execute([FromQuery] GetEarningsCallsRequest request, CancellationToken cancellationToken)
    {
        var vms = await _mediator.Send(request, cancellationToken);
        return Ok(vms);
    }
}

public class GetEarningsCallsRequest : EarningsCallsFilter, IRequest<IReadOnlyCollection<EarningCallVm>>
{
}

public class GetEarningsCallsRequestHandler : IRequestHandler<GetEarningsCallsRequest, IReadOnlyCollection<EarningCallVm>>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;

    public GetEarningsCallsRequestHandler(DatabaseContext database, ILinkCreator linkCreator)
    {
        _database = database;
        _linkCreator = linkCreator;
    }

    public async Task<IReadOnlyCollection<EarningCallVm>> Handle(GetEarningsCallsRequest request, CancellationToken cancellationToken)
    {
        var date = DateOnly.FromDateTime(DateTime.Now);
        var query = _database.From<EarningCall>()
            .AsNoTracking()
            .Where(x => x.Date == date);

        if (request.OnlyIncreased)
            query = query.Where(x => x.Surprise != null)
                .Where(x => x.Surprise!.Value > 0)
                .Where(x => x.Growth != null)
                .Where(x => x.Growth!.Value > 0);

        if (!string.IsNullOrWhiteSpace(request.Text))
            query = query.Where(x => EF.Functions.Like(x.Name, $"%{request.Text}%")
                                     || EF.Functions.Like(x.Symbol, $"%{request.Text}%"));

        var vms = await query.MapToVm()
            .OrderBy(x => x.Release)
            .ToListAsync(cancellationToken);

        foreach (var vm in vms)
            if (vm.Sources.HasFlag(InformationSource.AktienFinder))
                vm.LogoUrl = _linkCreator.CreateLogoUrl(vm.Symbol);

        return vms;
    }
}