using dominikz.Api.Utils;
using dominikz.Domain.Enums.Trades;
using dominikz.Domain.Filter;
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
public class SearchEarningsCalls : EndpointController
{
    private readonly IMediator _mediator;

    public SearchEarningsCalls(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Execute([FromQuery] SearchEarningsCallsRequest request, CancellationToken cancellationToken)
    {
        var vms = await _mediator.Send(request, cancellationToken);
        return Ok(vms);
    }
}

public class SearchEarningsCallsRequest : EarningsCallsFilter, IRequest<IReadOnlyCollection<EarningCallVm>>
{
}

public class SearchEarningsCallsRequestHandler : IRequestHandler<SearchEarningsCallsRequest, IReadOnlyCollection<EarningCallVm>>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;

    public SearchEarningsCallsRequestHandler(DatabaseContext database, ILinkCreator linkCreator)
    {
        _database = database;
        _linkCreator = linkCreator;
    }

    public async Task<IReadOnlyCollection<EarningCallVm>> Handle(SearchEarningsCallsRequest request, CancellationToken cancellationToken)
    {
        var date = request.Date ?? DateOnly.FromDateTime(DateTime.Now);
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