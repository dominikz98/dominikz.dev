using dominikz.Domain.ViewModels.Trading;
using dominikz.Infrastructure.Clients.Api;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Pages.Trading;

public partial class TradeDetail
{
    [Parameter] public int? Id { get; set; }

    [Inject] protected TradesEndpoints? Endpoints { get; set; }

    private TradeDetailVm? _data;

    protected override async Task OnInitializedAsync()
    {
        if (Id is null)
            return;

        _data = await Endpoints!.GetById(Id.Value);
    }
}