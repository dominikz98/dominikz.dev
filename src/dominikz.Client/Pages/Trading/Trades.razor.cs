using System.Drawing;
using dominikz.Client.Components;
using dominikz.Domain.Enums.Trades;
using dominikz.Domain.Filter;
using dominikz.Domain.ViewModels.Trading;
using dominikz.Infrastructure.Clients.Api;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Pages.Trading;

public partial class Trades
{
    [Inject] protected TradesEndpoints? TradesEndpoints { get; set; }
    [Inject] protected DownloadEndpoints? DownloadEndpoints { get; set; }

    private List<EarningCallVm> _earningsCalls = new();
    private List<Timeline.TimelineEvent> _events = new();

    protected override async Task OnInitializedAsync()
    {
        var calls = await TradesEndpoints!.SearchEarningsCalls(new EarningsCallsFilter());
        _earningsCalls = calls.Where(x => x.Growth != null)
            .OrderBy(x => x.Growth)
            .ToList();

        _events = calls.Where(x => x.Growth == null)
            .Select(x => new Timeline.TimelineEvent()
            {
                Date = x.Date.ToDateTime(x.Release ?? new TimeOnly(13, 0, 0)),
                Description = x.Name,
                Title = x.Symbol,
                SymbolSrc = x.Sources.HasFlag(InformationSource.AktienFinder) ? x.LogoUrl : null
            }).OrderBy(x => x.Date)
            .ToList();
    }

    private void SelectEarningCall(int id)
    {
    }
}