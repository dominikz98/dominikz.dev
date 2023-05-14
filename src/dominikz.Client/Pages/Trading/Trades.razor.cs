using dominikz.Client.Api;
using dominikz.Domain.Filter;
using dominikz.Domain.ViewModels.Trading;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace dominikz.Client.Pages.Trading;

public partial class Trades
{
    [Inject] protected TradesEndpoints? Endpoints { get; set; }
    [Inject] protected NavigationManager? NavManager { get; set; }
    [Inject] protected IJSRuntime? JsRuntime { get; set; }

    private IReadOnlyCollection<EarningCallListVm> _data = Array.Empty<EarningCallListVm>();
    private EarningCallVm? _selected;
    private bool _isMobileDevice;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        _isMobileDevice = await JsRuntime!.InvokeAsync<bool>("isMobileDevice");
    }

    protected override async Task OnInitializedAsync()
    {
        _data = await Endpoints!.SearchCalls(new EarningsCallsFilter() { Date = DateOnly.FromDateTime(DateTime.UtcNow) });
        var firstId = _data.OrderBy(x => x.IsReleased).ThenBy(x => x.Symbol).FirstOrDefault()?.Id;
        if (firstId == null)
            return;

        _selected = await Endpoints!.GetCallById(firstId.Value);
    }

    private async Task OnCallClicked(EarningCallListVm call)
    {
        if (_isMobileDevice)
        {
            NavManager!.NavigateTo($"/trades/earningcall/{call.Id}");
            return;
        }

        _selected = await Endpoints!.GetCallById(call.Id);
    }
}